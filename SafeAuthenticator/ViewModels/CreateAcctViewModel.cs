using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels
{
    internal class CreateAcctViewModel : BaseViewModel
    {
        private string _acctSecret;
        private string _acctPassword;
        private string _confirmAcctSecret;
        private string _confirmAcctPassword;
        private bool _isUiEnabled;
        private StrengthIndicator _locationStrength;
        private StrengthIndicator _passwordStrength;

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

        private string _acctSecretStrengthErrorMsg;

        public string AcctSecretStrengthErrorMsg
        {
            get => _acctSecretStrengthErrorMsg;
            set => SetProperty(ref _acctSecretStrengthErrorMsg, value);
        }

        private string _acctPasswordErrorMsg;

        public string AcctPasswordErrorMsg
        {
            get => _acctPasswordErrorMsg;
            set => SetProperty(ref _acctPasswordErrorMsg, value);
        }

        private string _acctPasswordStrengthErrorMsg;

        public string AcctPasswordStrengthErrorMsg
        {
            get => _acctPasswordStrengthErrorMsg;
            set => SetProperty(ref _acctPasswordStrengthErrorMsg, value);
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
                            AcctSecretStrengthErrorMsg = string.Empty;
                        }
                        return !string.IsNullOrWhiteSpace(AcctSecret) && !string.IsNullOrWhiteSpace(ConfirmAcctSecret);
                    }
                case 1:
                    {
                        if (AcctPassword == ConfirmAcctPassword)
                        {
                            AcctPasswordErrorMsg = string.Empty;
                            AcctPasswordStrengthErrorMsg = string.Empty;
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
                if (CarouselPagePosition == 1)
                {
                    if (AcctSecret != ConfirmAcctSecret)
                    {
                        AcctSecretErrorMsg = "Secret doesn't match";
                        ((Command)CarouselContinueCommand).ChangeCanExecute();
                        return;
                    }

                    using (NativeProgressDialog.ShowNativeDialog("Checking secret strength"))
                    {
                        await Task.Run(() =>
                        {
                            _locationStrength = Utilities.StrengthChecker(AcctSecret);
                            if (_locationStrength.Guesses < Constants.StrengthScoreWeak)
                            {
                                AcctSecretStrengthErrorMsg = "Secret needs to be stronger";
                                throw new InvalidOperationException();
                            }
                            AcctSecretStrengthErrorMsg = string.Empty;
                        });
                    }
                }

                if (CarouselPagePosition < 2)
                {
                    CarouselPagePosition += 1;
                }
                else
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
                if (ex.ErrorCode == -116)
                    CarouselPagePosition = 0;
                else if (ex.ErrorCode == -102)
                    CarouselPagePosition = 1;

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
                await Task.Run(() =>
                {
                    _passwordStrength = Utilities.StrengthChecker(AcctPassword);
                    if (_passwordStrength.Guesses < Constants.StrengthScoreSomeWhatSecure)
                    {
                        AcctPasswordStrengthErrorMsg = "Password needs to be stronger";
                        throw new InvalidOperationException();
                    }
                    AcctPasswordStrengthErrorMsg = string.Empty;
                });

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
