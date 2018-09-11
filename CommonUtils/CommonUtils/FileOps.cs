#if __MOBILE__
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommonUtils;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileOps))]

namespace CommonUtils {
  public class FileOps : IFileOps {
    public string ConfigFilesPath {
      get {
        string path;
#if __IOS__
// Resources -> /Library
        path = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
#elif __ANDROID__
        // Personal -> /data/data/@PACKAGE_NAME@/files
        path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
#endif
        return path;
      }
    }

    public async Task TransferAssetsAsync(List<(string, string)> fileList) {
      foreach (var tuple in fileList) {
#if __IOS__
        using (var reader = new StreamReader(Path.Combine(".", tuple.Item1))) {
#elif __ANDROID__
        using (var reader = new StreamReader(Android.App.Application.Context.Assets.Open(tuple.Item1))) {
#endif
          using (var writer = new StreamWriter(Path.Combine(ConfigFilesPath, tuple.Item2))) {
            await writer.WriteAsync(await reader.ReadToEndAsync());
            writer.Close();
          }

          reader.Close();
        }
      }
    }
  }
}
#endif
