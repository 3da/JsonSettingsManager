using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JsonSettingsManager
{
    public class FsProvider : IFsProvider
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string LoadTextFile(string path, Encoding encoding)
        {
            return File.ReadAllText(path, encoding);
        }

        public byte[] LoadBinFile(string path)
        {
            return File.ReadAllBytes(path);
        }

    }
}
