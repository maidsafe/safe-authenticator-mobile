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
using SafeAuthenticatorApp.Helpers;
using SafeAuthenticatorApp.Models;
using SafeAuthenticatorApp.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(NodeConnectionFileManager))]

namespace SafeAuthenticatorApp.Services
{
    public class NodeConnectionFileManager
    {
        private static string ConfigFilePath => DependencyService.Get<IFileOps>().ConfigFilesPath;

        private static readonly string _nodeConnectionFilePreferenceKey = "NodeConnectionFileList";
        private static readonly string _defaultNodeConnectionFileName = "node_connection_info.config";

        private List<NodeConnectionFile> _nodeConnectionFileList;

        public NodeConnectionFileManager()
        {
            _ = GetAllFileEntries();
        }

        internal (NodeConnectionFile, bool) AddNewNodeConnectionConfigFile(string fileName, string fileData)
        {
            var fileId = Convert.ToInt32(DateTime.Now.ToString("MMddmmssff"));
            File.WriteAllText(Path.Combine(ConfigFilePath, $"{fileId}.config"), fileData);

            var connecitonFile = new NodeConnectionFile
            {
                FileName = fileName,
                FileId = fileId,
                AddedOn = DateTime.Now.ToUniversalTime()
            };
            _nodeConnectionFileList.Add(connecitonFile);

            UpdateListInDevicePreferenceStore();

            if (_nodeConnectionFileList.Count == 1)
                return (connecitonFile, true);

            return (connecitonFile, false);
        }

        internal void DeleteNodeConnectionConfigFile(int fileId)
        {
            var fileEntry = _nodeConnectionFileList.FirstOrDefault(t => t.FileId == fileId);
            if (fileEntry == null)
                return;

            if (fileEntry.IsActive)
                File.Delete(Path.Combine(ConfigFilePath, _defaultNodeConnectionFileName));

            _nodeConnectionFileList.Remove(fileEntry);
            File.Delete(Path.Combine(ConfigFilePath, $"{fileId}.config"));
            UpdateListInDevicePreferenceStore();
        }

        internal void SetAsActiveConnectionConfigFile(int fileId)
        {
            var currentActiveFile = _nodeConnectionFileList.Where(t => t.IsActive);
            if (currentActiveFile != null && currentActiveFile.Any())
            {
                foreach (var item in currentActiveFile)
                {
                    item.IsActive = false;
                }
            }

            var fileEntry = _nodeConnectionFileList.FirstOrDefault(t => t.FileId == fileId);
            if (fileEntry == null)
                return;

            fileEntry.IsActive = true;
            File.Delete(Path.Combine(ConfigFilePath, _defaultNodeConnectionFileName));
            var fileData = File.ReadAllText(Path.Combine(ConfigFilePath, $"{fileId}.config"));
            File.WriteAllText(Path.Combine(ConfigFilePath, _defaultNodeConnectionFileName), fileData);
            UpdateListInDevicePreferenceStore();
        }

        internal bool ActiveConnectionConfigFileExists()
        {
            var configFiles = Directory.GetFiles(ConfigFilePath, _defaultNodeConnectionFileName);
            if (configFiles.Count() > 0)
                return true;
            else
                return false;
        }

        internal List<NodeConnectionFile> GetAllFileEntries()
        {
            if (_nodeConnectionFileList == null)
                _nodeConnectionFileList = new List<NodeConnectionFile>();

            string storedList;
            if (Preferences.ContainsKey(_nodeConnectionFilePreferenceKey))
                storedList = Preferences.Get(_nodeConnectionFilePreferenceKey, string.Empty);
            else
                storedList = string.Empty;

            if (!string.IsNullOrWhiteSpace(storedList))
                _nodeConnectionFileList = JsonConvert.DeserializeObject<List<NodeConnectionFile>>(storedList);

            return _nodeConnectionFileList;
        }

        internal void DeleteAllFiles()
        {
            var configFiles = Directory.GetFiles(ConfigFilePath, "*.config");
            foreach (var file in configFiles)
            {
                File.Delete(file);
            }

            if (Preferences.ContainsKey(_nodeConnectionFilePreferenceKey))
                Preferences.Remove(_nodeConnectionFilePreferenceKey);

            _nodeConnectionFileList.Clear();
        }

        private void UpdateListInDevicePreferenceStore()
        {
            if (Preferences.ContainsKey(_nodeConnectionFilePreferenceKey))
                Preferences.Remove(_nodeConnectionFilePreferenceKey);

            var serializedList = JsonConvert.SerializeObject(_nodeConnectionFileList);
            Preferences.Set(_nodeConnectionFilePreferenceKey, serializedList);
        }
    }
}
