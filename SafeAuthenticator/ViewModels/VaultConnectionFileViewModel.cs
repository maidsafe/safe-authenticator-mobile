using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public ObservableCollection<VaultConnectionFile> VaultConnectionFiles { get; set; }

        public VaultConnectionFileViewModel()
        {
            AddNewVaultConnectionFileCommand = new Command(async () => await PickFileFromTheDeviceAsync());
            VaultConnectionFiles = new ObservableCollection<VaultConnectionFile>(
                VaultConnectionFileManager.GetAllFileEntries());
        }

        private async Task PickFileFromTheDeviceAsync()
        {
            try
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null)
                    return; // user canceled file picking

                string fileName = fileData.FileName;
                string contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray);

                Debug.WriteLine("File name chosen: " + fileName);
                Debug.WriteLine("File data: " + contents);

                var friendlyFileName = await Application.Current.MainPage.DisplayPromptAsync(
                    "File name",
                    "Provide a friendly file name to identify between files",
                    maxLength: 15,
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
