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
            if (lastDataSource is FileDataSource f)
            {
                WorkDir = WorkDir ?? f.WorkDir;
                Encoding = Encoding ?? f.Encoding;
            }

            if (Encoding == null)
                Encoding = Encoding.UTF8;


            var searchPaths = new List<string>
            {
                Path
            };

            if (mode == LoadMode.Json)
                searchPaths.Add(Path + ".json");

            if (WorkDir != null)
            {
                searchPaths.Insert(0, System.IO.Path.Combine(WorkDir, Path));
                if (mode == LoadMode.Json)
                    searchPaths.Insert(1, System.IO.Path.Combine(WorkDir, Path + ".json"));
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

            WorkDir = bestSearchPath.DirectoryName;

            switch (mode)
            {
                case LoadMode.Json:
                    return (JToken.Parse(File.ReadAllText(bestSearchPath.FullName, Encoding)), this);
                case LoadMode.Text:
                    return (new JValue(File.ReadAllText(bestSearchPath.FullName, Encoding)), this);
                case LoadMode.Bin:
                    return (JToken.FromObject(File.ReadAllBytes(bestSearchPath.FullName)), this);
                case LoadMode.Lines:
                    var lines = File.ReadAllLines(bestSearchPath.FullName, Encoding);
                    return (new JArray(lines), this);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
