using System;
using System.Windows.Input;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Native;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels
{
    internal class LoginViewModel : BaseViewModel
    {
        private string _accountPassword;
        private string _accountSecret;
        private bool _isUiEnabled;

        public string AccountPassword
        {
            get => _accountPassword;
            set
            {
                SetProperty(ref _accountPassword, value);
                ((Command)LoginCommand).ChangeCanExecute();
            }
        }

        public string AccountSecret
        {
            get => _accountSecret;
            set
            {
                SetProperty(ref _accountSecret, value);
                ((Command)LoginCommand).ChangeCanExecute();
            }
        }

        public ICommand CreateAccountCommand { get; }

        public ICommand LoginCommand { get; }

        public ICommand NeedHelpCommand { get; }

        public bool IsUiEnabled
        {
            get => _isUiEnabled;
            set => SetProperty(ref _isUiEnabled, value);
        }

        public LoginViewModel()
        {
            Authenticator.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Authenticator.IsLogInitialised))
                {
                    IsUiEnabled = Authenticator.IsLogInitialised;
                }
            };

            IsUiEnabled = Authenticator.IsLogInitialised;

            CreateAccountCommand = new Command(OnCreateAcct);
            LoginCommand = new Command(OnLogin, CanExecute);
            CreateAccountCommand = new Command(OnCreateAcct);
            NeedHelpCommand = new Command(() =>
            {
                OpeNativeBrowserService.LaunchNativeEmbeddedBrowser(Constants.FaqUrl);
            });
        }

        private bool CanExecute()
        {
            return !string.IsNullOrWhiteSpace(AccountSecret) && !string.IsNullOrWhiteSpace(AccountPassword);
        }

        private void OnCreateAcct()
        {
            MessagingCenter.Send(this, MessengerConstants.NavCreateAcctPage);
        }

        private async void OnLogin()
        {
            try
            {
                using (NativeProgressDialog.ShowNativeDialog("Logging in"))
                {
                    await Authenticator.LoginAsync(AccountSecret, AccountPassword);
                    MessagingCenter.Send(this, MessengerConstants.NavHomePage);
                }
            }
            catch (FfiException ex)
            {
                var errorMessage = Utilities.GetErrorMessage(ex);
                await Application.Current.MainPage.DisplayAlert("Login", errorMessage, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Log in Failed: {ex.Message}", "OK");
            }
        }
    }
}
