using System.IO;
using JsonSettingsManager.DataSources;
using Newtonsoft.Json;

namespace JsonSettingsManager.Serialization
{
    public interface IWriter
    {
        void Write(string path, params Stream[] streams);
        IDataSource CreateDataSource(string path);
    }
}