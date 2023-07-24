using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JsonSettingsManager.DataSources;
using JsonSettingsManager.TypeResolving;
using Newtonsoft.Json;

namespace JsonSettingsManager.Serialization
{
    public class FileSystemWriter : IWriter
    {
        public void Write(string path, params Stream[] streams)
        {
            var newPath = path;

            var fi = new FileInfo(newPath);

            var dir = fi.Directory;

            if (!dir.Exists)
                dir.Create();



            using (var outputStream = File.Create(path))
            {
                foreach (var stream in streams)
                {
                    stream.CopyTo(outputStream);
                }
            }
        }

        public IDataSource CreateDataSource(string path)
        {
            return new FileDataSource()
            {
                Path = path
            };
        }
    }
}
