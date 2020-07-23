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
using SafeAuthenticatorApp.Helpers;
using SafeAuthenticatorApp.Models;
using Xamarin.Forms;

namespace SafeAuthenticatorApp.ViewModels
{
    internal class CreateAcctViewModel : BaseViewModel
    {
        private string _acctSecret;
        private string _acctPassword;
        private string _confirmAcctSecret;
        private string _confirmAcctPassword;
        private bool _isUiEnabled;

        public string AcctSecret
        {
            get => _acctSecret;
            set
            {
                SetProperty(ref _acctSecret, value);
                ((Command)CarouselContinueCommand).ChangeCanExecute();
            }
        }

        public string AcctPassword
        {
            get => _acctPassword;
            set
            {
                SetProperty(ref _acctPassword, value);
                ((Command)CarouselContinueCommand).ChangeCanExecute();
            }
        }

        public string ConfirmAcctSecret
        {
            get => _confirmAcctSecret;
            set
            {
                SetProperty(ref _confirmAcctSecret, value);
                ((Command)CarouselContinueCommand).ChangeCanExecute();
            }
        }

        public string ConfirmAcctPassword
        {
            get => _confirmAcctPassword;
            set
            {
                SetProperty(ref _confirmAcctPassword, value);
                ((Command)CarouselContinueCommand).ChangeCanExecute();
            }
        }

        public bool IsUiEnabled
        {
            get => _isUiEnabled;
            set => SetProperty(ref _isUiEnabled, value);
        }

        private int _carouselPagePosition;

        public int CarouselPagePosition
        {
            get => _carouselPagePosition;
            set
            {
                SetProperty(ref _carouselPagePosition, value);
                OnPropertyChanged(nameof(IsBackButtonVisible));
            }
        }

        public bool IsBackButtonVisible => CarouselPagePosition > 0;

        private string _acctSecretErrorMsg;

        public string AcctSecretErrorMsg
        {
            get => _acctSecretErrorMsg;
            set => SetProperty(ref _acctSecretErrorMsg, value);
        }

        private string _acctPasswordErrorMsg;

        public string AcctPasswordErrorMsg
        {
            get => _acctPasswordErrorMsg;
            set => SetProperty(ref _acctPasswordErrorMsg, value);
        }

        public ICommand CarouselPageChangeCommand { get; }

        public ICommand ClipboardPasteCommand { get; }

        public ICommand CarouselContinueCommand { get; }

        public ICommand CarouselBackCommand { get; }

        public ICommand ClaimTokenCommand { get; }

        public ICommand OpenForumLinkCommand { get; }

        public CreateAcctViewModel()
        {
            Authenticator.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Authenticator.IsLogInitialised))
                {
                    IsUiEnabled = Authenticator.IsLogInitialised;
                }
            };

            IsUiEnabled = Authenticator.IsLogInitialised;

            CarouselContinueCommand = new Command(async () => await OnContinueAsync(), CanExecute);
            CarouselBackCommand = new Command(OnBack);
            CarouselPageChangeCommand = new Command(CarouselPageChange);
            OpenForumLinkCommand = new Command(() =>
            {
                OpeNativeBrowserService.LaunchNativeEmbeddedBrowser(Constants.ForumLinkUrl);
            });
            ClaimTokenCommand = new Command(() =>
            {
                OpeNativeBrowserService.LaunchNativeEmbeddedBrowser(Constants.ClaimTokenUrl);
            });
        }

        private bool CanExecute()
        {
            switch (CarouselPagePosition)
            {
                case 0:
                    {
                        if (AcctSecret == ConfirmAcctSecret)
                        {
                            AcctSecretErrorMsg = string.Empty;
                        }
                        return !string.IsNullOrWhiteSpace(AcctSecret) && !string.IsNullOrWhiteSpace(ConfirmAcctSecret);
                    }
                case 1:
                    {
                        if (AcctPassword == ConfirmAcctPassword)
                        {
                            AcctPasswordErrorMsg = string.Empty;
                        }
                        return !string.IsNullOrWhiteSpace(AcctPassword) && !string.IsNullOrWhiteSpace(ConfirmAcctPassword);
                    }
                default:
                    return true;
            }
        }

        private async Task OnContinueAsync()
        {
            try
            {
                if (CarouselPagePosition == 0)
                {
                    if (AcctSecret != ConfirmAcctSecret)
                    {
                        AcctSecretErrorMsg = "Passphrase doesn't match";
                        ((Command)CarouselContinueCommand).ChangeCanExecute();
                        return;
                    }
                }

                if (CarouselPagePosition < 1)
                {
                    CarouselPagePosition += 1;
                }
                else if (CarouselPagePosition == 1)
                {
                    if (AcctPassword != ConfirmAcctPassword)
                    {
                        AcctPasswordErrorMsg = "Password doesn't match";
                        ((Command)CarouselContinueCommand).ChangeCanExecute();
                        return;
                    }
                    await CreateAcct();
                }
            }
            catch (FfiException ex)
            {
                var errorMessage = Utilities.GetErrorMessage(ex);
                if (ex.ErrorCode == -102)
                    CarouselPagePosition = 0;

                await Application.Current.MainPage.DisplayAlert("Create account", errorMessage, "OK");
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Create Acct Failed: {ex.Message}", "OK");
            }
        }

        private async Task CreateAcct()
        {
            using (NativeProgressDialog.ShowNativeDialog("Creating account"))
            {
                await Authenticator.CreateAccountAsync(AcctSecret, AcctPassword);
                MessagingCenter.Send(this, MessengerConstants.NavHomePage);
            }
        }

        private void OnBack()
        {
            CarouselPagePosition -= 1;
        }

        private void CarouselPageChange()
        {
            ((Command)CarouselContinueCommand).ChangeCanExecute();
        }
    }
}
