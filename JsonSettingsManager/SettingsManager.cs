using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using JsonSettingsManager.DataSources;
using JsonSettingsManager.SpecificProcessors;
using JsonSettingsManager.TypeResolving;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager
{
    public class SettingsManager
    {
        private readonly List<ISpecificProcessor> _specialProcessors;


        public SettingsManager(params ISpecificProcessor[] processors)
        {
            _specialProcessors = new List<ISpecificProcessor>
            {
                new LoadProcessor(),
                new MergeProcessor(),
                new MergeArrayProcessor(),
                new ConcatArrayProcessor(),
                new UnionArrayProcessor(),
                new ExceptArrayProcessor(),

            }.Concat(processors).ToList();
        }

        internal JToken LoadSettings(IDataSource dataSource, ParseContext context, LoadMode mode)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            JToken jToken;
            (jToken, dataSource) = dataSource.Load(context.DataSource, mode, context);

            return ProcessJToken(jToken, new ParseContext
            {
                DataSource = dataSource,
                Manager = this,
                Serializer = JsonSerializer.Create(new JsonSerializerSettings()
                {
                    Converters = new List<JsonConverter>()
                    {
                        new JsonImplConverter()
                    }
                })
            });
        }

        public JToken LoadSettings(IDataSource dataSource)
        {
            var parseContext = new ParseContext()
            {
                Manager = this,
                DataSource = dataSource
            };

            return LoadSettings(dataSource, parseContext, LoadMode.Json);
        }

        public JToken LoadSettings(string path, string workDir = null)
        {
            var fi = new FileInfo(path);
            if (fi.Extension == ".zip")
                return LoadSettingsZip(path, workDir);
            else
                return LoadSettingsJson(path, workDir);
        }

        public T LoadSettings<T>(string path, string workDir = null)
        {
            var fi = new FileInfo(path);
            if (fi.Extension == ".zip")
                return LoadSettingsZip<T>(path, workDir);
            else
                return LoadSettingsJson<T>(path, workDir);
        }

        public JToken LoadSettingsJson(string path, string workDir = null)
        {
            return LoadSettings(new FileDataSource() { Path = path, WorkDir = workDir });
        }

        public T LoadSettingsJson<T>(string path, string workDir = null)
        {
            var token = LoadSettingsJson(path, workDir);

            return LoadSettings<T>(token);
        }

        public T LoadSettings<T>(JToken token)
        {
            return token.ToObject<T>(JsonSerializer.Create(new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                    new JsonImplConverter()
                }
            }));
        }

        public JToken LoadSettingsZip(string path, string workDir = null)
        {
            using (var stream = File.OpenRead(path))
            using (var archive = new ZipArchive(stream))
            {
                return LoadSettings(new ZipDataSource() { Path = "Main.json", ZipArchive = archive });
            }
        }

        public T LoadSettingsZip<T>(string path, string workDir = null)
        {
            var token = LoadSettingsZip(path, workDir);

            return LoadSettings<T>(token);
        }

        private JToken ProcessJToken(JToken jToken, ParseContext context)
        {
            if (jToken is JObject jObject)
                return ProcessJObject(jObject, context);
            if (jToken is JArray jArray)
                return ProcessJArray(jArray, context);

            return jToken;
        }

        private JArray ProcessJArray(JArray jArray, ParseContext context)
        {
            var anotherArray = new JArray();
            foreach (var token in jArray.Children())
            {
                context.MergeArray = false;
                var p = ProcessJToken(token, context);

                if (p == null)
                    continue;

                if (context.MergeArray)
                {
                    foreach (var child in p.Children())
                    {
                        anotherArray.Add(child);
                    }
                }
                else
                    anotherArray.Add(p);
            }

            return anotherArray;
        }

        private JToken ProcessJObject(JObject jObject, ParseContext context)
        {
            var specialProperties = jObject.Properties().Where(q => q.Name.StartsWith("@"))
                .Select(q => new { Name = q.Name.Substring(1), Token = q })
                .Select(q => new
                {
                    q.Name,
                    q.Token,
                    Processor = _specialProcessors.FirstOrDefault(proc =>
                        proc.IsPrefix
                            ? q.Name.StartsWith(proc.KeyWord)
                            : q.Name.Equals(proc.KeyWord, StringComparison.OrdinalIgnoreCase))
                })
                .Where(q => q.Processor != null).ToList();


            foreach (var specialProperty in specialProperties.Select(q => q.Token))
            {
                specialProperty.Remove();
            }

            JToken result = jObject;

            foreach (var specialProperty in specialProperties)
            {
                var jobj = result as JObject;
                if (jobj == null)
                    break;
                result = specialProperty.Processor.Do(context, specialProperty.Token.Value, jobj, specialProperty.Name);
            }

            var jobj2 = result as JObject;

            if (jobj2 != null)
            {
                foreach (var property in jobj2.Properties())
                {
                    property.Value = ProcessJToken(property.Value, context);
                }
            }




            return result;
        }

    }
}
