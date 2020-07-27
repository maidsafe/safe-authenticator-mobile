// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SafeApp.Core;
using SafeAuthenticatorApp.Helpers;
using Xamarin.Forms;

namespace SafeAuthenticatorApp.ViewModels
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

        public async Task SetVaultConnectionConfigFileDirAsync()
        {
            await Authenticator.SetConfigFileDirectoryPathAsync();
        }

        public bool VaultConnectionFileExists()
        {
            return VaultConnectionFileManager.ActiveConnectionConfigFileExists();
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
