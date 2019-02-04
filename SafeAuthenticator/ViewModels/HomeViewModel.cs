using System;
using System.Linq;
using System.Windows.Input;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels
{
    internal class HomeViewModel : BaseViewModel
    {
        private bool _isRefreshing;

        public ICommand RefreshAccountsCommand { get; }

        public ICommand SettingsCommand { get; }

        public ObservableRangeCollection<RegisteredAppModel> Apps { get; set; }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            private set => SetProperty(ref _isRefreshing, value);
        }

        private RegisteredAppModel _selectedRegisteredAccount;

        public RegisteredAppModel SelectedRegisteredAccount
        {
            get => _selectedRegisteredAccount;

            set
            {
                if (value == null)
                {
                    return;
                }

                OnAccountSelected(value);
                SetProperty(ref _selectedRegisteredAccount, value);
            }
        }

        public HomeViewModel()
        {
            IsRefreshing = false;
            Apps = new ObservableRangeCollection<RegisteredAppModel>();
            RefreshAccountsCommand = new Command(OnRefreshAccounts);
            SettingsCommand = new Command(OnSettings);
            Device.BeginInvokeOnMainThread(OnRefreshAccounts);

            MessagingCenter.Subscribe<AppInfoViewModel>(this, MessengerConstants.RefreshHomePage, (sender) => { OnRefreshAccounts(); });
        }

        ~HomeViewModel()
        {
            MessagingCenter.Unsubscribe<AppInfoViewModel>(this, MessengerConstants.RefreshHomePage);
        }

        private void OnAccountSelected(RegisteredAppModel appModelInfo)
        {
            MessagingCenter.Send(this, MessengerConstants.NavAppInfoPage, appModelInfo);
        }

        private void OnSettings()
        {
            MessagingCenter.Send(this, MessengerConstants.NavSettingsPage);
        }

        private async void OnRefreshAccounts()
        {
            try
            {
                IsRefreshing = true;
                var registeredApps = await Authenticator.GetRegisteredAppsAsync();
                registeredApps = registeredApps.OrderBy(a => a.AppName).ToList();
                Apps.ReplaceRange(registeredApps);
            }
            catch (FfiException ex)
            {
                var errorMessage = Utilities.GetErrorMessage(ex);
                await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Refresh Accounts Failed: {ex.Message}", "OK");
            }
            IsRefreshing = false;
        }

        public async void HandleAuthenticationReq()
        {
            if (string.IsNullOrEmpty(Authenticator.AuthenticationReq))
            {
                return;
            }

            await Authenticator.HandleUrlActivationAsync(Authenticator.AuthenticationReq);
            Authenticator.AuthenticationReq = null;
        }
    }
}
