using System;
using System.Windows.Input;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Native;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels
{
    internal class SettingsViewModel : BaseViewModel
    {
        public ICommand LogoutCommand { get; }

        public ICommand FaqCommand { get; }

        public ICommand PrivacyInfoCommand { get; }

        public string ApplicationVersion => AppInfo.VersionString;

        private string _accountStatus;

        public string AccountStorageInfo
        {
            get => _accountStatus;
            set => SetProperty(ref _accountStatus, value);
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public bool AuthReconnect
        {
            get => Authenticator.AuthReconnect;
            set
            {
                if (Authenticator.AuthReconnect != value)
                {
                    Authenticator.AuthReconnect = value;
                    if (AuthReconnect)
                    {
                        OnAutoReconnect();
                    }
                }

                OnPropertyChanged();
            }
        }

        private async void OnAutoReconnect()
        {
            AuthReconnect = await Application.Current.MainPage.DisplayAlert(
            "Auto Reconnect",
            Constants.AutoReconnectInfoDialog,
            "OK",
            "Cancel");
        }

        public SettingsViewModel()
        {
            AccountStorageInfo = Preferences.Get(nameof(AccountStorageInfo), "--");
            LogoutCommand = new Command(OnLogout);

            FaqCommand = new Command(() =>
            {
                OpeNativeBrowserService.LaunchNativeEmbeddedBrowser(Constants.FaqUrl);
            });

            PrivacyInfoCommand = new Command(() =>
            {
                OpeNativeBrowserService.LaunchNativeEmbeddedBrowser(Constants.PrivacyInfoUrl);
            });
        }

        public async void GetAccountInfo()
        {
            try
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    throw new Exception(Constants.NoInternetMessage);
                }
                IsBusy = true;
                var acctStorageTuple = await Authenticator.GetAccountInfoAsync();
                AccountStorageInfo = $"{acctStorageTuple.Item1} / {acctStorageTuple.Item2}";
                Preferences.Set(nameof(AccountStorageInfo), AccountStorageInfo);
            }
            catch (FfiException ex)
            {
                var errorMessage = Utilities.GetErrorMessage(ex);
                await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Fetching account info failed: {ex.Message}", "OK");
            }
            IsBusy = false;
        }

        private async void OnLogout()
        {
            try
            {
                if (await Application.Current.MainPage.DisplayAlert(
                    "Logout",
                    "Are you sure you want to logout?",
                    "Logout",
                    "Cancel"))
                {
                    using (NativeProgressDialog.ShowNativeDialog("Logging out"))
                    {
                        Preferences.Remove(nameof(AccountStorageInfo));
                        await Authenticator.LogoutAsync();
                        MessagingCenter.Send(this, MessengerConstants.NavLoginPage);
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Log out Failed: {ex.Message}", "OK");
            }
        }
    }
}
