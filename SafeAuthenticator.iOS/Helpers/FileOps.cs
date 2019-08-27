using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.iOS.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileOps))]

namespace SafeAuthenticator.iOS.Helpers
{
    public class FileOps : IFileOps
    {
        public string ConfigFilesPath
        {
            get
            {
                // Resources -> /Library
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
                return path;
            }
        }

        public async Task TransferAssetsAsync(List<(string, string)> fileList)
        {
            foreach (var tuple in fileList)
            {
                using (var reader = new StreamReader(Path.Combine(".", tuple.Item1)))
                {
                    using (var writer = new StreamWriter(Path.Combine(ConfigFilesPath, tuple.Item2)))
                    {
                        await writer.WriteAsync(await reader.ReadToEndAsync());
                        writer.Close();
                    }

                    reader.Close();
                }
            }
        }
    }
}
