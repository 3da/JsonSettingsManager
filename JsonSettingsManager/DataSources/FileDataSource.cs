using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.DataSources
{
    public class FileDataSource : IDataSource
    {
        public string Path { get; set; }

        public string WorkDir { get; set; }

        public Encoding Encoding { get; set; }

        public (JToken, IDataSource) Load(IDataSource lastDataSource, LoadMode mode, ParseContext context)
        {
            var newDataSource = new FileDataSource()
            {
                Encoding = Encoding,
                Path = Path,
                WorkDir = WorkDir
            };
            if (lastDataSource is FileDataSource f)
            {
                newDataSource.WorkDir = WorkDir ?? f.WorkDir;
                newDataSource.Encoding = Encoding ?? f.Encoding;
            }

            if (newDataSource.Encoding == null)
                newDataSource.Encoding = Encoding.UTF8;


            var searchPaths = new List<string>
            {
                newDataSource.Path
            };

            if (mode == LoadMode.Json)
                searchPaths.Add(newDataSource.Path + ".json");

            if (newDataSource.WorkDir != null)
            {
                searchPaths.Insert(0, System.IO.Path.Combine(newDataSource.WorkDir, Path));
                if (mode == LoadMode.Json)
                    searchPaths.Insert(1, System.IO.Path.Combine(newDataSource.WorkDir, Path + ".json"));
            }

            FileInfo bestSearchPath = null;

            foreach (var searchPath in searchPaths)
            {
                var sp = new FileInfo(searchPath);
                if (sp.Exists)
                {
                    bestSearchPath = sp;
                    break;
                }
            }
            if (bestSearchPath == null)
                throw new Exception();

            newDataSource.WorkDir = bestSearchPath.DirectoryName;

            switch (mode)
            {
                case LoadMode.Json:
                    return (JToken.Parse(File.ReadAllText(bestSearchPath.FullName, newDataSource.Encoding)), newDataSource);
                case LoadMode.Text:
                    return (new JValue(File.ReadAllText(bestSearchPath.FullName, newDataSource.Encoding)), newDataSource);
                case LoadMode.Bin:
                    return (JToken.FromObject(File.ReadAllBytes(bestSearchPath.FullName)), newDataSource);
                case LoadMode.Lines:
                    var lines = File.ReadAllLines(bestSearchPath.FullName, newDataSource.Encoding);
                    return (new JArray(lines), newDataSource);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
