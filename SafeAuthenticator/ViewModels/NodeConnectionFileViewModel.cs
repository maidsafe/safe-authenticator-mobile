// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using SafeAuthenticatorApp.Helpers;
using SafeAuthenticatorApp.Models;
using Xamarin.Forms;

namespace SafeAuthenticatorApp.ViewModels
{
    internal class NodeConnectionFileViewModel : BaseViewModel
    {
        private string _nodeS3DownloadLink = "https://sn-node-config.s3.eu-west-2.amazonaws.com/shared-section/node_connection_info.config";

        private string _defaultNodeFileName = "MaidSafe hosted network";

        public ICommand AddNewNodeConnectionFileCommand { get; }

        public ICommand DeleteAllNodeConnectionFilesCommand { get;  }

        public ICommand NodeConnectionFileSelectionCommand { get;  }

        public ICommand DownloadMaidSafeNodeCommand { get; }

        public ICommand SetActiveFileCommand { get; }

        private NodeConnectionFile _selectedFile;

        public NodeConnectionFile SelectedFile
        {
            get => _selectedFile;
            set => SetProperty(ref _selectedFile, value);
        }

        private ObservableCollection<NodeConnectionFile> _nodeConnectionFiles;

        public ObservableCollection<NodeConnectionFile> NodeConnectionFiles
        {
            get => _nodeConnectionFiles;
            set => SetProperty(ref _nodeConnectionFiles, value);
        }

        public NodeConnectionFileViewModel()
        {
            AddNewNodeConnectionFileCommand = new Command(async () => await PickFileFromTheDeviceAsync());
            DeleteAllNodeConnectionFilesCommand = new Command(async () => await DeleteAllNodeFilesAsync());
            NodeConnectionFileSelectionCommand = new Command(async (object fileId) => await ShowFileSelectionOptionsAsync(fileId));
            SetActiveFileCommand = new Command(async (object fileId) => await SetActiveNodeFileAsync(fileId));
            DownloadMaidSafeNodeCommand = new Command(async () => await DownloadAndUseMaidSafeNodeConnectionFileAsync());
            RefreshNodeConnectionFilesList();
        }

        private async Task DownloadAndUseMaidSafeNodeConnectionFileAsync()
        {
            try
            {
                using (NativeProgressDialog.ShowNativeDialog("Download network connection file"))
                {
                    using (var client = new HttpClient())
                    {
                        using (var response = await client.GetAsync(_nodeS3DownloadLink))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var fileContent = await response.Content.ReadAsStringAsync();
                                if (fileContent.Length > 0)
                                {
                                    var (newlyAddedNodeFile, isFirst) =
                                        NodeConnectionFileManager.AddNewNodeConnectionConfigFile(
                                            _defaultNodeFileName,
                                            fileContent);

                                    if (newlyAddedNodeFile != null)
                                        await SetActiveNodeFileAsync(newlyAddedNodeFile.FileId);

                                    RefreshNodeConnectionFilesList();
                                }
                            }
                            else
                            {
                                throw new Exception("Failed to download file from S3.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Download network connection file",
                    "Failed to download the network connection file.",
                    "ok");
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task SetActiveNodeFileAsync(object fileId)
        {
            if (Authenticator.IsLoggedIn)
            {
                var result = await Application.Current.MainPage.DisplayAlert(
                    "Network connection file",
                    "You'll be logged out of the app.",
                    "Continue",
                    "Cancel");

                if (result)
                {
                    SetNewDefaultNodeFile((int)fileId);
                    RefreshNodeConnectionFilesList();
                    using (NativeProgressDialog.ShowNativeDialog("Logging out"))
                    {
                        await Authenticator.LogoutAsync();
                        MessagingCenter.Send(this, MessengerConstants.NavLoginPage);
                    }
                }
            }
            else
            {
                SetNewDefaultNodeFile((int)fileId);
                RefreshNodeConnectionFilesList();
            }
        }

        private void RefreshNodeConnectionFilesList()
        {
            if (Device.RuntimePlatform == Device.iOS && NodeConnectionFiles != null)
            {
                NodeConnectionFiles.Clear();
            }

            NodeConnectionFiles = new ObservableCollection<NodeConnectionFile>(
            NodeConnectionFileManager.GetAllFileEntries());
            if (NodeConnectionFiles != null)
                SelectedFile = NodeConnectionFiles.FirstOrDefault(f => f.IsActive);
        }

        private async Task ShowFileSelectionOptionsAsync(object fileId)
        {
            var deletedSelected = await Application.Current.MainPage.DisplayAlert(
                "Choose a network",
                "Do you want to delete network connection file.",
                "Delete",
                "Cancel");

            if (deletedSelected)
            {
                var nodeFile = NodeConnectionFiles.FirstOrDefault(f => f.FileId == (int)fileId);
                var activeFile = NodeConnectionFiles.FirstOrDefault(f => f.IsActive);
                var deletingActiveFile = false;

                if (activeFile.FileId == (int)fileId)
                    deletingActiveFile = true;

                if (nodeFile != null)
                    DeleteNodeFileAsync((int)fileId);

                DeleteNodeFileAsync((int)fileId);

                if (deletingActiveFile && NodeConnectionFiles.Count > 0)
                    await SetActiveNodeFileAsync(NodeConnectionFiles[0].FileId);
            }
            SelectedFile = null;
        }

        private async Task DeleteAllNodeFilesAsync()
        {
            try
            {
                if (NodeConnectionFiles.Count == 0)
                    return;

                var result = await Application.Current.MainPage.DisplayAlert(
                    "Delete network connection files",
                    "Do you want to delete all available network connection files?",
                    "Delete all",
                    "Cancel");

                if (result)
                {
                    NodeConnectionFileManager.DeleteAllFiles();
                    NodeConnectionFiles.Clear();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Delete network connection files",
                    "Failed to delete the valid files",
                    "ok");
                Debug.WriteLine(ex.Message);
            }
        }

        private void DeleteNodeFileAsync(int fileId)
        {
            try
            {
                NodeConnectionFileManager.DeleteNodeConnectionConfigFile(fileId);
                var fileEntry = NodeConnectionFiles.FirstOrDefault(f => f.FileId == fileId);
                if (fileEntry != null)
                    RefreshNodeConnectionFilesList();
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Delete network connection file",
                    "Failed to delete selected network connection file.",
                    "Ok");
                Debug.WriteLine(ex.Message);
            }
        }

        private void SetNewDefaultNodeFile(int fileId)
        {
            try
            {
                NodeConnectionFileManager.SetAsActiveConnectionConfigFile(fileId);
                Task.Run(async () => await Authenticator.SetConfigFileDirectoryPathAsync());
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Choose a network",
                    "Failed to choose a network to connect.",
                    "Ok");
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task PickFileFromTheDeviceAsync()
        {
            try
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null)
                    return;

                string fileName = fileData.FileName;
                string contents = Encoding.UTF8.GetString(fileData.DataArray);
                var friendlyFileName = await Application.Current.MainPage.DisplayPromptAsync(
                    "Add network connection file",
                    "Provide a file name to identify between different networks.",
                    placeholder: "file name",
                    maxLength: 25,
                    keyboard: Keyboard.Text,
                    initialValue: string.Empty);

                if (string.IsNullOrEmpty(friendlyFileName))
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Add network connection file",
                        "Failed to save the connection file. Filename required.",
                        "Ok");
                    return;
                }

                var (newlyAddedNodeFile, isFirst) =
                    NodeConnectionFileManager.AddNewNodeConnectionConfigFile(friendlyFileName, contents);

                if (newlyAddedNodeFile != null)
                    await SetActiveNodeFileAsync(newlyAddedNodeFile.FileId);

                RefreshNodeConnectionFilesList();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Add network connection file",
                    "Failed to save a new connection file.",
                    "Ok");
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
