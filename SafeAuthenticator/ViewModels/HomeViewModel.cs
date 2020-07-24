// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SafeApp.Core;
using SafeAuthenticator;
using SafeAuthenticatorApp.Helpers;
using SafeAuthenticatorApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeAuthenticatorApp.ViewModels
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
            set => SetProperty(ref _isRefreshing, value);
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
            Apps = new ObservableRangeCollection<RegisteredAppModel>();
            RefreshAccountsCommand = new Command(async () => await OnRefreshAccounts(), () => !IsRefreshing);
            SettingsCommand = new Command(OnSettings);

            MessagingCenter.Subscribe<AppInfoViewModel>(this, MessengerConstants.RefreshHomePage, async (sender) => { await OnRefreshAccounts(); });
            MessagingCenter.Subscribe<RequestDetailViewModel, IpcReq>(this, MessengerConstants.RefreshHomePage, (sender, decodeResult) =>
            {
                var decodedType = decodeResult.GetType();

                // Authentication Request
                if (decodedType == typeof(AuthIpcReq))
                {
                    var ipcReq = (AuthIpcReq)decodeResult;
                    var app = new RegisteredAppModel(
                        ipcReq.AuthReq.App,
                        ipcReq.AuthReq.Containers,
                        new AppPermissions
                        {
                            PerformMutations = ipcReq.AuthReq.AppPermissionPerformMutations,
                            TransferCoins = ipcReq.AuthReq.AppPermissionTransferCoins,
                            GetBalance = ipcReq.AuthReq.AppPermissionGetBalance,
                        });
                    var isAppContainerRequested = ipcReq.AuthReq.AppContainer;
                    var appOwnContainer = new ContainerPermissionsModel()
                    {
                        ContainerName = Constants.AppOwnFormattedContainer,
                        Access = new PermissionSetModel
                        {
                            Read = true,
                            Insert = true,
                            Update = true,
                            Delete = true,
                            ManagePermissions = true
                        }
                    };

                    // Add app to registeredAppList if not present
                    if (!Apps.Contains(app))
                    {
                        // Adding app's own container if present
                        if (isAppContainerRequested)
                        {
                            app.Containers.Add(appOwnContainer);
                            app.Containers.ReplaceRange(app.Containers.OrderBy(a => a.ContainerName).ToObservableRangeCollection());
                        }
                        var registeredApps = Apps;
                        registeredApps.Add(app);
                        registeredApps = registeredApps.OrderBy(a => a.AppName).ToObservableRangeCollection();
                        Apps.ReplaceRange(registeredApps);
                    }
                    else
                    {
                        // If app already exists in registeredAppList, and app's own container is requested but not previously added
                        if (isAppContainerRequested)
                        {
                            var registeredAppsItem = Apps.FirstOrDefault(a => a.AppId == app.AppId);
                            var container = registeredAppsItem.Containers.FirstOrDefault(a => a.ContainerName == Constants.AppOwnFormattedContainer);
                            if (container == null)
                            {
                                registeredAppsItem.Containers.Add(appOwnContainer);
                                registeredAppsItem.Containers.ReplaceRange(registeredAppsItem.Containers.OrderBy(a => a.ContainerName).ToObservableRangeCollection());
                            }
                        }
                    }
                }
                else if (decodedType == typeof(ContainersIpcReq))
                {
                    // Container Request
                    var ipcReq = (ContainersIpcReq)decodeResult;
                    var app = new RegisteredAppModel(
                                            ipcReq.ContainersReq.App,
                                            ipcReq.ContainersReq.Containers,
                                            null);

                    var registeredAppsItem = Apps.FirstOrDefault(a => a.AppId == app.AppId);
                    foreach (var container in app.Containers)
                    {
                        var containersItem = registeredAppsItem.Containers.FirstOrDefault(a => a.ContainerName == container.ContainerName);

                        // If requested container not present add new else update permission set of existing container
                        if (containersItem == null)
                            registeredAppsItem.Containers.Add(container);
                        else
                            containersItem.Access = container.Access;
                    }
                    registeredAppsItem.Containers.ReplaceRange(registeredAppsItem.Containers.OrderBy(a => a.ContainerName).ToObservableRangeCollection());
                }
            });

            Device.InvokeOnMainThreadAsync(OnRefreshAccounts);
        }

        ~HomeViewModel()
        {
            MessagingCenter.Unsubscribe<AppInfoViewModel>(this, MessengerConstants.RefreshHomePage);
            MessagingCenter.Unsubscribe<RequestDetailViewModel, RegisteredAppModel>(this, MessengerConstants.RefreshHomePage);
        }

        private void OnAccountSelected(RegisteredAppModel appModelInfo)
        {
            MessagingCenter.Send(this, MessengerConstants.NavAppInfoPage, appModelInfo);
        }

        private void OnSettings()
        {
            MessagingCenter.Send(this, MessengerConstants.NavSettingsPage);
        }

        private async Task OnRefreshAccounts()
        {
            try
            {
                IsRefreshing = true;
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    throw new Exception(Constants.NoInternetMessage);
                }
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
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsRefreshing = false;
                });
            }
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
