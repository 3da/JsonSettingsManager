using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.DataSources
{
    public class ZipDataSource : IDataSource
    {
        [JsonIgnore]
        public ZipArchive ZipArchive { get; set; }

        public string Path { get; set; }

        public Encoding Encoding { get; set; }

        public (JToken, IDataSource) Load(IDataSource lastDataSource, LoadMode mode, ParseContext context)
        {
            if (lastDataSource is ZipDataSource f)
            {
                ZipArchive = ZipArchive ?? f.ZipArchive;
                Encoding = Encoding ?? f.Encoding;
            }


            if (Encoding == null)
                Encoding = Encoding.UTF8;

            ZipArchiveEntry entry;

            if (ZipArchive == null)
            {
                var stream = File.OpenRead(Path);
                ZipArchive = new ZipArchive(stream);

                entry = ZipArchive.GetEntry("Main.json");
            }
            else
                entry = ZipArchive.GetEntry(Path);

            using (var stream = entry.Open())
            {
                string text = null;
                if (mode == LoadMode.Json || mode == LoadMode.Text)
                    text = new StreamReader(stream, Encoding).ReadToEnd();
                switch (mode)
                {
                    case LoadMode.Json:
                        return (JToken.Parse(text), this);
                    case LoadMode.Text:
                        return (new JValue(text), this);
                    case LoadMode.Bin:
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            return (JToken.FromObject(memoryStream.ToArray()), this);
                        }
                    case LoadMode.Lines:
                        var reader = new StreamReader(stream, Encoding);
                        var lines = new List<string>();
                        while (true)
                        {
                            var line = reader.ReadLine();
                            if (line == null)
                                break;

                            lines.Add(line);

                        }
                        return (new JArray(lines), this);
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }



        }
    }
}
