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
                }

                OnPropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            AccountStorageInfo = Preferences.Get(nameof(AccountStorageInfo), "--");
            LogoutCommand = new Command(OnLogout);

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
                IsBusy = true;
                var acctStorageTuple = await Authenticator.GetAccountInfoAsync();
                AccountStorageInfo = $"{acctStorageTuple.Item1} / {acctStorageTuple.Item2}";
                Preferences.Set(nameof(AccountStorageInfo), AccountStorageInfo);
                IsBusy = false;
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
            if (await Application.Current.MainPage.DisplayAlert(
                "Logout",
                "Are you sure you want to logout?",
                "Logout",
                "Cancel"))
            {
                AuthReconnect = false;
                Preferences.Remove(nameof(AccountStorageInfo));
                await Authenticator.LogoutAsync();
                MessagingCenter.Send(this, MessengerConstants.NavLoginPage);
            }
        }
    }
}
