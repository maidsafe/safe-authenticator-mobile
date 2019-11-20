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

        internal VaultConnectionFile AddNewVaultConnectionConfigFile(string friendlyFileName, byte[] fileData)
        {
            var fileId = Convert.ToInt32(DateTime.Now.ToString("MMddmmssff"));
            File.WriteAllBytes(Path.Combine(ConfigFilePath, $"{fileId}.config"), fileData);

            var connecitonFile = new VaultConnectionFile
            {
                FiendlyFileName = friendlyFileName,
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
            UpdateListInDevicePreferenceStore();
            File.Delete(Path.Combine(ConfigFilePath, $"{fileId}.config"));
        }

        internal void SetAsActiveConnectionConfigFile(int fileId)
        {
            var fileEntry = _vaultConnectionFileList.FirstOrDefault(t => t.FileId == fileId);
            if (fileEntry == null)
                return;

            File.Delete(Path.Combine(ConfigFilePath, _defaultVaultConnectionFileName));
            var fileData = File.ReadAllBytes(Path.Combine(ConfigFilePath, $"{fileId}.config"));
            File.WriteAllBytes(Path.Combine(ConfigFilePath, _defaultVaultConnectionFileName), fileData);
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
            if (Preferences.ContainsKey(_vaultConnectionFilePreferenceKey))
                Preferences.Remove(_vaultConnectionFilePreferenceKey);

            var configFiles = Directory.GetFiles(ConfigFilePath, ".config");
            foreach (var file in configFiles)
            {
                File.Delete(file);
            }
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
