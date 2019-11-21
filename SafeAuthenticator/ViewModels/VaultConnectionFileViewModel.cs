using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using SafeAuthenticator.Models;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels
{
    internal class VaultConnectionFileViewModel : BaseViewModel
    {
        public ICommand AddNewVaultConnectionFileCommand { get; }

        public ICommand DeleteAllVaultConnectionFilesCommand { get;  }

        public ICommand VaultConnectionFileSelectionCommand { get;  }

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
            DeleteAllVaultConnectionFilesCommand = new Command(DeleteAllVaultFiles);
            VaultConnectionFileSelectionCommand = new Command(async () => await ShowFileSelectionOptionsAsync());
            RefreshVaultConnectionFilesList();
        }

        private void RefreshVaultConnectionFilesList()
        {
            VaultConnectionFiles = new ObservableCollection<VaultConnectionFile>(
                VaultConnectionFileManager.GetAllFileEntries());
        }

        private async Task ShowFileSelectionOptionsAsync()
        {
            string action = await Application.Current.MainPage.DisplayActionSheet("Vault Connection File Options", "Cancel", "Delete", "Activate");

            switch (action)
            {
                case "Delete":
                    DeleteVaultFileAsync(SelectedFile.FileId);
                    break;
                case "Activate":
                    SetNewDefaultVaultFile(SelectedFile.FileId);
                    RefreshVaultConnectionFilesList();
                    break;
            }
        }

        private void DeleteAllVaultFiles(object obj)
        {
            try
            {
                if (VaultConnectionFiles.Count == 0)
                    return;

                VaultConnectionFileManager.DeleteAllFiles();
                VaultConnectionFiles.Clear();
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Delete Vault Connection Files",
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
                    VaultConnectionFiles.Remove(fileEntry);
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Activate Vault File",
                    "Failed to activate the vault connection file",
                    "ok");
                Debug.WriteLine(ex.Message);
            }
        }

        private void SetNewDefaultVaultFile(int fileId)
        {
            try
            {
                VaultConnectionFileManager.SetAsActiveConnectionConfigFile(fileId);
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Delete Vault File",
                    "Failed to delete the valid file",
                    "ok");
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
                string contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray);

                var friendlyFileName = await Application.Current.MainPage.DisplayPromptAsync(
                    "File name",
                    "Provide a friendly file name to identify between files",
                    maxLength: 25,
                    keyboard: Keyboard.Text);

                if (string.IsNullOrEmpty(friendlyFileName))
                {
                    await Application.Current.MainPage.DisplayAlert(
                    "File picker",
                    "Failed to save the file. Filename required.",
                    "ok");
                    return;
                }

                var connectionFile = VaultConnectionFileManager.AddNewVaultConnectionConfigFile(friendlyFileName, fileData.DataArray);

                VaultConnectionFiles.Add(connectionFile);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "File picker",
                    "Failed to pickup a valid file",
                    "ok");
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
