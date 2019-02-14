using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
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
        private string _invitation;
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

        public string Invitation
        {
            get => _invitation;
            set
            {
                SetProperty(ref _invitation, value);
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
                OpeNativeBrowserService.LaunchNativeEmbeddedBrowser(@"https://safenetforum.org/t/trust-level-1-basic-user-requirements/15200");
            });
            ClaimTokenCommand = new Command(() =>
            {
                OpeNativeBrowserService.LaunchNativeEmbeddedBrowser(@"https://invite.maidsafe.net/");
            });
            ClipboardPasteCommand = new Command(async () =>
            {
                Invitation = await Clipboard.GetTextAsync();
            });
        }

        private bool CanExecute()
        {
            switch (CarouselPagePosition)
            {
                case 0:
                    return !string.IsNullOrWhiteSpace(Invitation);
                case 1:
                    return !string.IsNullOrWhiteSpace(AcctSecret) && !string.IsNullOrWhiteSpace(ConfirmAcctSecret) &&
                        AcctSecret == ConfirmAcctSecret;
                case 2:
                    return !string.IsNullOrWhiteSpace(AcctPassword) && !string.IsNullOrWhiteSpace(ConfirmAcctPassword) &&
                        AcctPassword == ConfirmAcctPassword;
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
                    using (UserDialogs.Instance.Loading("Checking secret strength"))
                    {
                        await Task.Run(() =>
                        {
                            _locationStrength = Utilities.StrengthChecker(AcctSecret);
                            if (_locationStrength.Guesses < Constants.AccStrengthWeak)
                            {
                                AcctSecretErrorMsg = "Secret needs to be stronger";
                                throw new InvalidOperationException();
                            }
                            AcctSecretErrorMsg = string.Empty;
                        });
                    }
                }

                if (CarouselPagePosition < 2)
                    CarouselPagePosition += 1;
                else
                    await CreateAcct();
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
            using (UserDialogs.Instance.Loading("Creating account"))
            {
                await Task.Run(() =>
                {
                    _passwordStrength = Utilities.StrengthChecker(AcctPassword);
                    if (_passwordStrength.Guesses < Constants.AccStrengthSomeWhatSecure)
                    {
                        AcctPasswordErrorMsg = "Password needs to be stronger";
                        throw new InvalidOperationException();
                    }
                    AcctPasswordErrorMsg = string.Empty;
                });

                await Authenticator.CreateAccountAsync(AcctSecret, AcctPassword, Invitation);
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
