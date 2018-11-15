using System;
using System.Windows.Input;
using SafeAuthenticator.Helpers;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels {
  internal class LoginViewModel : BaseViewModel {
    private string _acctPassword;
    private string _acctSecret;
    private bool _isUiEnabled;

    public string AcctPassword { get => _acctPassword; set {
                SetProperty(ref _acctPassword, value);
                ((Command)LoginCommand).ChangeCanExecute();
            }
        }

    public string AcctSecret { get => _acctSecret; set {
                SetProperty(ref _acctSecret, value);
                ((Command)LoginCommand).ChangeCanExecute();
            }
       }

    public ICommand CreateAcctCommand { get; }

    public ICommand LoginCommand { get; }

    public bool IsUiEnabled { get => _isUiEnabled; set => SetProperty(ref _isUiEnabled, value); }

    public bool AuthReconnect {
      get => Authenticator.AuthReconnect;
      set {
        if (Authenticator.AuthReconnect != value) {
          Authenticator.AuthReconnect = value;
        }

        OnPropertyChanged();
      }
    }

    public LoginViewModel() {
      Authenticator.PropertyChanged += (s, e) => {
        if (e.PropertyName == nameof(Authenticator.IsLogInitialised)) {
          IsUiEnabled = Authenticator.IsLogInitialised;
        }
      };

      IsUiEnabled = Authenticator.IsLogInitialised;

      CreateAcctCommand = new Command(OnCreateAcct);

      LoginCommand = new Command(OnLogin, CanExecute);

      AcctSecret = string.Empty;
      AcctPassword = string.Empty;
    }

    private bool CanExecute()
    {
        return !string.IsNullOrWhiteSpace(AcctPassword) && !string.IsNullOrWhiteSpace(AcctSecret);
    }

     private void OnCreateAcct() {
      MessagingCenter.Send(this, MessengerConstants.NavCreateAcctPage);
    }

    private async void OnLogin() {
      IsUiEnabled = false;
      try {
        await Authenticator.LoginAsync(AcctSecret, AcctPassword);
        MessagingCenter.Send(this, MessengerConstants.NavHomePage);
      } catch (Exception ex) {
        await Application.Current.MainPage.DisplayAlert("Error", $"Log in Failed: {ex.Message}", "OK");
        IsUiEnabled = true;
      }
    }
  }
}
