using System;
using System.Linq;
using System.Windows.Input;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels {
  internal class HomeViewModel : BaseViewModel {
    private string _accountStorageInfo;
    private bool _isRefreshing;
    public ICommand LogoutCommand { get; }
    public ObservableRangeCollection<RegisteredAppModel> Apps { get; set; }
    public bool IsRefreshing { get => _isRefreshing; private set => SetProperty(ref _isRefreshing, value); }
    public ICommand RefreshAccountsCommand { get; }
    public ICommand AccountSelectedCommand { get; }
    public string AccountStorageInfo { get => _accountStorageInfo; set => SetProperty(ref _accountStorageInfo, value); }

    public HomeViewModel() {
      IsRefreshing = false;
      Apps = new ObservableRangeCollection<RegisteredAppModel>();
      RefreshAccountsCommand = new Command(OnRefreshAccounts);
      AccountSelectedCommand = new Command<RegisteredAppModel>(OnAccountSelected);
      LogoutCommand = new Command(OnLogout);
      Device.BeginInvokeOnMainThread(OnRefreshAccounts);

      MessagingCenter.Subscribe<AppInfoViewModel>(this, MessengerConstants.RefreshHomePage,(sender) => {
        OnRefreshAccounts();
      });
    }

    private void OnAccountSelected(RegisteredAppModel appModelInfo) {
      MessagingCenter.Send(this, MessengerConstants.NavAppInfoPage, appModelInfo);
    }

    private async void OnLogout() {
      await Authenticator.LogoutAsync();
      MessagingCenter.Send(this, MessengerConstants.NavLoginPage);
    }

    private async void OnRefreshAccounts() {
      try {
        IsRefreshing = true;
        var registeredApps = await Authenticator.GetRegisteredAppsAsync();
        Apps.ReplaceRange(registeredApps);
        Apps.Sort();
        var acctStorageTuple = await Authenticator.GetAccountInfoAsync();
        AccountStorageInfo = $"{acctStorageTuple.Item1} / {acctStorageTuple.Item2}";
        IsRefreshing = false;
      } catch (Exception ex) {
        await Application.Current.MainPage.DisplayAlert("Error", $"Refresh Accounts Failed: {ex.Message}", "OK");
      }
    }
  }
}
