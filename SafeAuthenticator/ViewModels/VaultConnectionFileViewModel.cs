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
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels
{
    internal class VaultConnectionFileViewModel : BaseViewModel
    {
        private string _vaultS3DownloadLink = "https://safe-vault-config.s3.eu-west-2.amazonaws.com/shared-section/vault_connection_info.config";

        private string _defaultVaultFileName = "MaidSafe hosted section";

        public ICommand AddNewVaultConnectionFileCommand { get; }

        public ICommand DeleteAllVaultConnectionFilesCommand { get;  }

        public ICommand VaultConnectionFileSelectionCommand { get;  }

        public ICommand DownloadMaidSafeVaultCommand { get; }

        public ICommand SetActiveFileCommand { get; }

        private VaultConnectionFile _selectedFile;

        public VaultConnectionFile SelectedFile
        {
            get => _selectedFile;
            set => SetProperty(ref _selectedFile, value);
        }

        private ObservableCollection<VaultConnectionFile> _vaultConnectionFiles;

        public ObservableCollection<VaultConnectionFile> VaultConnectionFiles
        {
            get => _vaultConnectionFiles;
            set => SetProperty(ref _vaultConnectionFiles, value);
        }

        public VaultConnectionFileViewModel()
        {
            AddNewVaultConnectionFileCommand = new Command(async () => await PickFileFromTheDeviceAsync());
            DeleteAllVaultConnectionFilesCommand = new Command(async () => await DeleteAllVaultFilesAsync());
            VaultConnectionFileSelectionCommand = new Command(async (object fileId) => await ShowFileSelectionOptionsAsync(fileId));
            SetActiveFileCommand = new Command(async (object fileId) => await SetActiveVaultFileAsync(fileId));
            DownloadMaidSafeVaultCommand = new Command(async () => await DownloadAndUseMaidSafeVaultConnectionFileAsync());
            RefreshVaultConnectionFilesList();
        }

        private async Task DownloadAndUseMaidSafeVaultConnectionFileAsync()
        {
            try
            {
                using (NativeProgressDialog.ShowNativeDialog("Download vault connection file"))
                {
                    using (var client = new HttpClient())
                    {
                        using (var response = await client.GetAsync(_vaultS3DownloadLink))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var fileContent = await response.Content.ReadAsStringAsync();
                                if (fileContent.Length > 0)
                                {
                                    var (newlyAddedVaultFile, isFirst) =
                                        VaultConnectionFileManager.AddNewVaultConnectionConfigFile(
                                            _defaultVaultFileName,
                                            fileContent);

                                    if (newlyAddedVaultFile != null)
                                        await SetActiveVaultFileAsync(newlyAddedVaultFile.FileId);

                                    RefreshVaultConnectionFilesList();
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
                    "Download vault connection file",
                    "Failed to download the vault connection file.",
                    "ok");
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task SetActiveVaultFileAsync(object fileId)
        {
            if (Authenticator.IsLoggedIn)
            {
                var result = await Application.Current.MainPage.DisplayAlert(
                    "Vault connection file",
                    "You'll be logged out of the app.",
                    "Continue",
                    "Cancel");

                if (result)
                {
                    SetNewDefaultVaultFile((int)fileId);
                    RefreshVaultConnectionFilesList();
                    using (NativeProgressDialog.ShowNativeDialog("Logging out"))
                    {
                        await Authenticator.LogoutAsync();
                        MessagingCenter.Send(this, MessengerConstants.NavLoginPage);
                    }
                }
            }
            else
            {
                SetNewDefaultVaultFile((int)fileId);
                RefreshVaultConnectionFilesList();
            }
        }

        private void RefreshVaultConnectionFilesList()
        {
            if (Device.RuntimePlatform == Device.iOS && VaultConnectionFiles != null)
            {
                VaultConnectionFiles.Clear();
            }

            VaultConnectionFiles = new ObservableCollection<VaultConnectionFile>(
            VaultConnectionFileManager.GetAllFileEntries());
            if (VaultConnectionFiles != null)
                SelectedFile = VaultConnectionFiles.FirstOrDefault(f => f.IsActive);
        }

        private async Task ShowFileSelectionOptionsAsync(object fileId)
        {
            var deletedSelected = await Application.Current.MainPage.DisplayAlert(
                "Choose a vault",
                "Do you want to delete vault connection file.",
                "Delete",
                "Cancel");

            if (deletedSelected)
            {
                var vaultFile = VaultConnectionFiles.FirstOrDefault(f => f.FileId == (int)fileId);
                var activeFile = VaultConnectionFiles.FirstOrDefault(f => f.IsActive);
                var deletingActiveFile = false;

                if (activeFile.FileId == (int)fileId)
                    deletingActiveFile = true;

                if (vaultFile != null)
                    DeleteVaultFileAsync((int)fileId);

                DeleteVaultFileAsync((int)fileId);

                if (deletingActiveFile && VaultConnectionFiles.Count > 0)
                    await SetActiveVaultFileAsync(VaultConnectionFiles[0].FileId);
            }
            SelectedFile = null;
        }

        private async Task DeleteAllVaultFilesAsync()
        {
            try
            {
                if (VaultConnectionFiles.Count == 0)
                    return;

                var result = await Application.Current.MainPage.DisplayAlert(
                    "Delete vault connection files",
                    "Do you want to delete all available vault connection files?",
                    "Delete all",
                    "Cancel");

                if (result)
                {
                    VaultConnectionFileManager.DeleteAllFiles();
                    VaultConnectionFiles.Clear();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Delete vault connection files",
                    "Failed to delete the valid files",
                    "ok");
                Debug.WriteLine(ex.Message);
            }
        }

        private void DeleteVaultFileAsync(int fileId)
        {
            try
            {
                VaultConnectionFileManager.DeleteVaultConnectionConfigFile(fileId);
                var fileEntry = VaultConnectionFiles.FirstOrDefault(f => f.FileId == fileId);
                if (fileEntry != null)
                    RefreshVaultConnectionFilesList();
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Delete vault connection file",
                    "Failed to delete selected vault connection file.",
                    "Ok");
                Debug.WriteLine(ex.Message);
            }
        }

        private void SetNewDefaultVaultFile(int fileId)
        {
            try
            {
                VaultConnectionFileManager.SetAsActiveConnectionConfigFile(fileId);
                Task.Run(async () => await Authenticator.SetConfigFileDirectoryPathAsync());
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Choose a vault",
                    "Failed to choose a vault to connect.",
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
                    "Add vault connection file",
                    "Provide a file name to identify between different vault connection files.",
                    placeholder: "file name",
                    maxLength: 25,
                    keyboard: Keyboard.Text,
                    initialValue: string.Empty);

                if (string.IsNullOrEmpty(friendlyFileName))
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Add vault connection file",
                        "Failed to save the connection file. Filename required.",
                        "Ok");
                    return;
                }

                var (newlyAddedVaultFile, isFirst) =
                    VaultConnectionFileManager.AddNewVaultConnectionConfigFile(friendlyFileName, contents);

                if (newlyAddedVaultFile != null)
                    await SetActiveVaultFileAsync(newlyAddedVaultFile.FileId);

                RefreshVaultConnectionFilesList();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Add vault connection file",
                    "Failed to save a new connection file.",
                    "Ok");
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
