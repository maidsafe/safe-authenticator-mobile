using System;
using System.Windows.Input;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Native;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels
{
    internal class SettingsViewModel : BaseViewModel
    {
        public ICommand LogoutCommand { get; }

        public ICommand FaqCommand { get; }

        public ICommand PrivacyInfoCommand { get; }

        private string _accountStorageInfo;

        public string AccountStorageInfo
        {
            get => _accountStorageInfo;
            set => SetProperty(ref _accountStorageInfo, value);
        }

        public bool AuthReconnect
        {
            get => Authenticator.AuthReconnect;
            set
            {
                if (Authenticator.AuthReconnect != value)
                {
                    Authenticator.AuthReconnect = value;
                }

                OnPropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            LogoutCommand = new Command(OnLogout);
            AccountStorageInfo = "Fetching account info...";

            FaqCommand = new Command(() =>
            {
                OpeNativeBrowserService.LaunchNativeEmbeddedBrowser(@"https://safenetforum.org/t/safe-authenticator-faq/26683");
            });

            PrivacyInfoCommand = new Command(() =>
            {
                OpeNativeBrowserService.LaunchNativeEmbeddedBrowser(@"https://safenetwork.tech/privacy/");
            });
        }

        public async void GetAccountInfo()
        {
            try
            {
                var acctStorageTuple = await Authenticator.GetAccountInfoAsync();
                AccountStorageInfo = $"{acctStorageTuple.Item1} / {acctStorageTuple.Item2}";
            }
            catch (FfiException ex)
            {
                var errorMessage = Utilities.GetErrorMessage(ex);
                await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Log in Failed: {ex.Message}", "OK");
            }
        }

        private async void OnLogout()
        {
            var result = await Application.Current.MainPage.DisplayAlert(
                "Logout",
                "Are you sure you want to logout?",
                "Logout",
                "Cancel");

            if (result)
            {
                AuthReconnect = false;
                await Authenticator.LogoutAsync();
                MessagingCenter.Send(this, MessengerConstants.NavLoginPage);
            }
        }
    }
}
