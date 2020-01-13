// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

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
