using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SafeAuthenticator.Droid.Helpers;
using SafeAuthenticator.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileOps))]
namespace SafeAuthenticator.Droid.Helpers
{
    public class FileOps : IFileOps
    {
        public string ConfigFilesPath
        {
            get
            {
                // Personal -> /data/data/@PACKAGE_NAME@/files
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return path;
            }
        }

        public async Task TransferAssetsAsync(List<(string, string)> fileList)
        {
            foreach (var tuple in fileList)
            {
                using (var reader = new StreamReader(Android.App.Application.Context.Assets.Open(tuple.Item1)))
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
