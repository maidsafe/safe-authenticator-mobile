using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Native;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels
{
    internal class CreateAcctViewModel : BaseViewModel
    {
        private string _acctPassword;
        private string _acctSecret;
        private string _invitation;
        private bool _isUiEnabled;
        private (double, double, string) _locationStrength;
        private (double, double, string) _passwordStrength;

        public string AcctPassword
        {
            get => _acctPassword;
            set
            {
                SetProperty(ref _acctPassword, value);
                ((Command)CreateAcctCommand).ChangeCanExecute();
            }
        }

        public string AcctSecret
        {
            get => _acctSecret;
            set
            {
                SetProperty(ref _acctSecret, value);
                ((Command)CreateAcctCommand).ChangeCanExecute();
            }
        }

        public string Invitation
        {
            get => _invitation;
            set
            {
                SetProperty(ref _invitation, value);
                ((Command)CreateAcctCommand).ChangeCanExecute();
            }
        }

        public ICommand CreateAcctCommand { get; }

        public bool IsUiEnabled
        {
            get => _isUiEnabled;
            set => SetProperty(ref _isUiEnabled, value);
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

            CreateAcctCommand = new Command(OnCreateAcct, CanExecute);

            AcctSecret = string.Empty;
            AcctPassword = string.Empty;
            Invitation = string.Empty;
        }

        private bool CanExecute()
        {
            return !string.IsNullOrWhiteSpace(AcctPassword) && !string.IsNullOrWhiteSpace(AcctSecret) &&
                   !string.IsNullOrWhiteSpace(Invitation);
        }

        private async void OnCreateAcct()
        {
            try
            {
                using (UserDialogs.Instance.Loading("Loading"))
                {
                    await Task.Run(() =>
                    {
                        _locationStrength = Utilities.StrengthChecker(AcctSecret);
                        _passwordStrength = Utilities.StrengthChecker(AcctPassword);
                        if (_locationStrength.Item1 < AppConstants.AccStrengthWeak)
                            throw new Exception("Secret needs to be stronger");

                        if (_passwordStrength.Item1 < AppConstants.AccStrengthSomeWhatSecure)
                            throw new Exception("Password needs to be stronger");
                    });
                    await Authenticator.CreateAccountAsync(AcctSecret, AcctPassword, Invitation);
                    MessagingCenter.Send(this, MessengerConstants.NavHomePage);
                }
            }
            catch (FfiException ex)
            {
                var errorMessage = Utilities.GetErrorMessage(ex);
                await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Create Acct Failed: {ex.Message}", "OK");
            }
        }
    }
}
