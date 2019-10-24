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

        public IServiceProvider ServiceProvider { get; set; }

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

            };

            _specialProcessors.AddRange(processors);
        }

        private List<JsonConverter> Converters
        {
            get
            {
                var result = new List<JsonConverter>(2)
                {
                    new JsonImplConverter(ServiceProvider)
                };

                if (ServiceProvider != null)
                    result.Add(new DependencyInjectionActivator(ServiceProvider));

                return result;
            }
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
                DisableProcessors = context.DisableProcessors,
                Serializer = JsonSerializer.Create(new JsonSerializerSettings()
                {
                    Converters = Converters
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
                Converters = Converters
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
            try
            {
                if (jToken is JObject jObject)
                    return ProcessJObject(jObject, context);
                if (jToken is JArray jArray)
                    return ProcessJArray(jArray, context);

                return jToken;
            }
            catch (Exception e)
            {
                throw new SettingsException($"Error procesing JToken: {jToken.Path}", e)
                {
                    JToken = jToken
                };
            }

        }

        private JToken ProcessJArray(JArray jArray, ParseContext context)
        {
            var anotherArray = new JArray();
            foreach (var token in jArray.Children())
            {
                context.MergeArray = false;
                JToken p;
                p = ProcessJToken(token, context);

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

            if (context.DisableProcessors == false)
            {
                foreach (var specialProperty in specialProperties)
                {
                    var jobj = result as JObject;
                    if (jobj == null)
                        break;

                    var processedOptions = ProcessJToken(specialProperty.Token.Value, context);

                    try
                    {
                        result = specialProperty.Processor.Do(context, processedOptions, jobj, specialProperty.Name);
                    }
                    catch (Exception e)
                    {
                        throw new SettingsException($"Error processing {specialProperty.Name} ({jobj.Path})", e)
                        {
                            JToken = specialProperty.Token.Value
                        };
                    }


                }
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
