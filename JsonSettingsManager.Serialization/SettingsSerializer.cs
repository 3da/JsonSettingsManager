using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using JsonSettingsManager.DataSources;
using JsonSettingsManager.Serialization.Attributes;
using JsonSettingsManager.TypeResolving;
using Newtonsoft.Json;

namespace JsonSettingsManager.Serialization
{
    public class SettingsSerializer
    {
        public void SaveZip(object obj, string path)
        {
            using (var stream = File.Create(path))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                var context = new SerializationContext(new ZipEntryWriter(archive, new Uri(Directory.GetCurrentDirectory() + "\\")))
                {
                    GlobalContext = new GlobalContext()
                };

                context.SaveExternal += SaveExternal;
                try
                {
                    SaveExternal(context, new FileInfo("Main.json").FullName, LoadMode.Json, obj);
                }
                finally
                {
                    context.SaveExternal -= SaveExternal;
                }
            }

        }


        public void SaveJson(object obj, string path)
        {
            var context = new SerializationContext(new FileSystemWriter())
            {
                GlobalContext = new GlobalContext()
            };

            context.SaveExternal += SaveExternal;

            SaveExternal(context, path, LoadMode.Json, obj);

            context.SaveExternal -= SaveExternal;
        }

        public string SaveJsonString(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter>()
                {
                    new JsonImplConverter(null)
                },
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Formatting = Formatting.Indented
            });
        }

        static void SaveExternal(SerializationContext context, string path, LoadMode mode, object value)
        {
            var newPath = path;

            var fi = new FileInfo(newPath);

            var dir = fi.Directory;

            if (!dir.Exists)
                dir.Create();

            var newContext = new SerializationContext(context.Writer)
            {
                WorkDir = dir.FullName,
                GlobalContext = context.GlobalContext
            };

            newContext.SaveExternal += SaveExternal;

            var serializer = JsonSerializer.Create(new JsonSerializerSettings()
            {
                ContractResolver = new MySolver(newContext),
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter>()
                {
                    new JsonImplConverter(null)
                },
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Formatting = Formatting.Indented
            });

            switch (mode)
            {
                case LoadMode.Json:
                    using (var memoryStream = new MemoryStream())
                    {
                        var streamWriter = new StreamWriter(memoryStream);
                        serializer.Serialize(streamWriter, value);
                        streamWriter.Flush();
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        context.Writer.Write(newPath, memoryStream);
                    }
                    break;
                case LoadMode.Text:
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes((string)value)))
                        context.Writer.Write(newPath, stream);
                    break;
                case LoadMode.Bin:
                    using (var stream = new MemoryStream((byte[])value))
                        context.Writer.Write(newPath, stream);
                    break;
                case LoadMode.LargeBin:
                    var bytes = (byte[][])value;
                    var streams = bytes.Select(b => (Stream)new MemoryStream(b)).ToArray();
                    context.Writer.Write(newPath, streams);
                    foreach (var stream in streams)
                    {
                        stream.Dispose();
                    }
                    break;
                case LoadMode.Lines:
                    using (var memoryStream = new MemoryStream())
                    {
                        var streamWriter = new StreamWriter(memoryStream);
                        foreach (string s in (IEnumerable<string>)value)
                        {
                            streamWriter.WriteLine(s);
                        }
                        streamWriter.Flush();
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        context.Writer.Write(newPath, memoryStream);

                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }



            newContext.SaveExternal -= SaveExternal;
        }
    }
}
