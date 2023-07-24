using System.Text;

namespace JsonSettingsManager
{
    public interface IFsProvider
    {
        bool FileExists(string path);
        string LoadTextFile(string path, Encoding encoding);
        byte[] LoadBinFile(string path);
        byte[][] LoadLargeBinFile(string path);
    }
}