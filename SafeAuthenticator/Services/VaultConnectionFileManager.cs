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
using System.Linq;
using Newtonsoft.Json;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(VaultConnectionFileManager))]

namespace SafeAuthenticator.Services
{
    public class VaultConnectionFileManager
    {
        private static string ConfigFilePath => DependencyService.Get<IFileOps>().ConfigFilesPath;

        private static readonly string _vaultConnectionFilePreferenceKey = "VaultConnectionFileList";
        private static readonly string _defaultVaultConnectionFileName = "vault_connection_info.config";

        private List<VaultConnectionFile> _vaultConnectionFileList;

        public VaultConnectionFileManager()
        {
            _ = GetAllFileEntries();
        }

        internal VaultConnectionFile AddNewVaultConnectionConfigFile(string fileName, byte[] fileData)
        {
            var fileId = Convert.ToInt32(DateTime.Now.ToString("MMddmmssff"));
            File.WriteAllBytes(Path.Combine(ConfigFilePath, $"{fileId}.config"), fileData);

            var connecitonFile = new VaultConnectionFile
            {
                FileName = fileName,
                FileId = fileId,
                AddedOn = DateTime.Now.ToUniversalTime()
            };
            _vaultConnectionFileList.Add(connecitonFile);

            UpdateListInDevicePreferenceStore();

            return connecitonFile;
        }

        internal void DeleteVaultConnectionConfigFile(int fileId)
        {
            var fileEntry = _vaultConnectionFileList.FirstOrDefault(t => t.FileId == fileId);
            if (fileEntry == null)
                return;

            _vaultConnectionFileList.Remove(fileEntry);
            File.Delete(Path.Combine(ConfigFilePath, $"{fileId}.config"));
            UpdateListInDevicePreferenceStore();
        }

        internal void SetAsActiveConnectionConfigFile(int fileId)
        {
            var currentActiveFile = _vaultConnectionFileList.Where(t => t.IsActive);
            if (currentActiveFile != null && currentActiveFile.Any())
            {
                foreach (var item in currentActiveFile)
                {
                    item.IsActive = false;
                }
            }

            var fileEntry = _vaultConnectionFileList.FirstOrDefault(t => t.FileId == fileId);
            if (fileEntry == null)
                return;

            fileEntry.IsActive = true;
            File.Delete(Path.Combine(ConfigFilePath, _defaultVaultConnectionFileName));
            var fileData = File.ReadAllBytes(Path.Combine(ConfigFilePath, $"{fileId}.config"));
            File.WriteAllBytes(Path.Combine(ConfigFilePath, _defaultVaultConnectionFileName), fileData);
            UpdateListInDevicePreferenceStore();
        }

        internal bool ActiveConnectionConfigFileExists()
        {
            var configFiles = Directory.GetFiles(ConfigFilePath, _defaultVaultConnectionFileName);
            if (configFiles.Count() > 0)
                return true;
            else
                return false;
        }

        internal List<VaultConnectionFile> GetAllFileEntries()
        {
            if (_vaultConnectionFileList == null)
                _vaultConnectionFileList = new List<VaultConnectionFile>();

            string storedList;
            if (Preferences.ContainsKey(_vaultConnectionFilePreferenceKey))
                storedList = Preferences.Get(_vaultConnectionFilePreferenceKey, string.Empty);
            else
                storedList = string.Empty;

            if (!string.IsNullOrWhiteSpace(storedList))
                _vaultConnectionFileList = JsonConvert.DeserializeObject<List<VaultConnectionFile>>(storedList);

            return _vaultConnectionFileList;
        }

        internal void DeleteAllFiles()
        {
            var configFiles = Directory.GetFiles(ConfigFilePath, "*.config");
            foreach (var file in configFiles)
            {
                File.Delete(file);
            }

            if (Preferences.ContainsKey(_vaultConnectionFilePreferenceKey))
                Preferences.Remove(_vaultConnectionFilePreferenceKey);

            _vaultConnectionFileList.Clear();
        }

        private void UpdateListInDevicePreferenceStore()
        {
            if (Preferences.ContainsKey(_vaultConnectionFilePreferenceKey))
                Preferences.Remove(_vaultConnectionFilePreferenceKey);

            var serializedList = JsonConvert.SerializeObject(_vaultConnectionFileList);
            Preferences.Set(_vaultConnectionFilePreferenceKey, serializedList);
        }
    }
}
