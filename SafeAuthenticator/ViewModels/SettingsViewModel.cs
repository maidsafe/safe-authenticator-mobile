using System;
using System.Windows.Input;
using SafeAuthenticator.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels
{
    internal class SettingsViewModel : BaseViewModel
    {
        public ICommand LogoutCommand { get; }

        public ICommand FaqCommand { get; }

        public ICommand PrivacyInfoCommand { get; }

        public ICommand VauleCommandManagerCommand { get; }

        public string ApplicationVersion => AppInfo.VersionString;

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
            LogoutCommand = new Command(OnLogout);

            VauleCommandManagerCommand = new Command(() =>
            {
                MessagingCenter.Send(this, MessengerConstants.NavVaultConnectionManagerPage);
            });

            FaqCommand = new Command(() =>
            {
                OpeNativeBrowserService.LaunchNativeEmbeddedBrowser(Constants.FaqUrl);
            });

            PrivacyInfoCommand = new Command(() =>
            {
                OpeNativeBrowserService.LaunchNativeEmbeddedBrowser(Constants.PrivacyInfoUrl);
            });
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
