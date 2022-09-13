using System.IO;

namespace AzureAdLogsClient.Core
{
    public interface IFileHelper
    {
        void StoreFile(string path, string content);
    }
    
    public class FileHelper: IFileHelper
    {
        public void StoreFile(string path, string content)
        {
            File.Delete(path);
            using StreamWriter sw = File.AppendText(path);
            sw.Write(content);
        }
    }
}