using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSettingsManager.Tests
{
    public class TestFsProvider : IFsProvider
    {
        private readonly FsProvider _fsProvider = new FsProvider();

        public List<string> List1 { get; } = new List<string>();

        public bool FileExists(string path)
        {
            return _fsProvider.FileExists(path);
        }

        public string LoadTextFile(string path, Encoding encoding)
        {
            List1.Add(path);
            return _fsProvider.LoadTextFile(path, encoding);
        }

        public byte[] LoadBinFile(string path)
        {
            return _fsProvider.LoadBinFile(path);
        }
    }
}
