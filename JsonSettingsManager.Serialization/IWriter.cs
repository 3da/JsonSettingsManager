using System.IO;
using JsonSettingsManager.DataSources;
using Newtonsoft.Json;

namespace JsonSettingsManager.Serialization
{
    public interface IWriter
    {
        void Write(string path, Stream stream);
        IDataSource CreateDataSource(string path);
    }
}