// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using SafeApp.Core;
using SafeAuthenticator;
using SafeAuthenticatorApp.Controls;
using SafeAuthenticatorApp.Helpers;
using SafeAuthenticatorApp.Models;
using SafeAuthenticatorApp.Services;
using SafeAuthenticatorApp.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthService))]

namespace SafeAuthenticatorApp.Services
{
    public class AuthService : ObservableObject, IDisposable
    {
        private const string AutoReconnectPropKey = "AutoReconnect";
        private readonly SemaphoreSlim _reconnectSemaphore = new SemaphoreSlim(1, 1);
        private Authenticator _authenticator;
        private bool _isLogInitialised;
        private string _secret;
        private string _password;

        protected INativeProgressDialogService NativeProgressDialog => DependencyService.Get<INativeProgressDialogService>();

        public string AuthenticationReq { get; set; }

        public bool IsLoggedIn => _authenticator != null;

        internal bool IsLogInitialised
        {
            get => _isLogInitialised;
            private set => SetProperty(ref _isLogInitialised, value);
        }

        internal bool IsRevocationComplete
        {
            get => Preferences.Get(Constants.IsRevocationComplete, true);
            set => Preferences.Set(Constants.IsRevocationComplete, value);
        }

        private CredentialCacheService CredentialCache { get; }

        internal bool AuthReconnect
        {
            get
            {
                if (!Application.Current.Properties.ContainsKey(AutoReconnectPropKey))
                {
                    Application.Current.Properties[AutoReconnectPropKey] = false;
                    return false;
                }

                return (bool)Application.Current.Properties[AutoReconnectPropKey];
            }

            set
            {
                if (value)
                {
                    StoreCredentials();
                }
                else
                {
                    CredentialCache.Delete();
                }
                Application.Current.Properties[AutoReconnectPropKey] = value;
            }
        }

        public AuthService()
        {
            _isLogInitialised = false;
            CredentialCache = new CredentialCacheService();
            Connectivity.ConnectivityChanged += InternetConnectivityChanged;
            Task.Run(async () => await InitLoggingAsync());
        }

        public void Dispose()
        {
            FreeState();
            GC.SuppressFinalize(this);
        }

        internal async Task CheckAndReconnect()
        {
            await _reconnectSemaphore.WaitAsync();
            try
            {
                if (_authenticator == null)
                {
                    if (AuthReconnect)
                    {
                        var (location, password) = await CredentialCache.Retrieve();
                        using (NativeProgressDialog.ShowNativeDialog("Reconnecting to Network"))
                        {
                            await LoginAsync(location, password);
                            MessagingCenter.Send(this, MessengerConstants.NavHomePage);
                        }
                    }
                }
            }
            catch (FfiException ex)
            {
                var errorMessage = Utilities.GetErrorMessage(ex);
                await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Unable to Reconnect: {ex.Message}", "OK");
                FreeState();
                MessagingCenter.Send(this, MessengerConstants.ResetAppViews);
            }
            finally
            {
                _reconnectSemaphore.Release(1);
            }
        }

        internal async Task SetConfigFileDirectoryPathAsync()
        {
            var fileOps = DependencyService.Get<IFileOps>();
            await Authenticator.AuthSetConfigurationFilePathAsync(fileOps.ConfigFilesPath);
            Debug.WriteLine($"Set config dir path to : {fileOps.ConfigFilesPath}");
        }

        internal async Task CreateAccountAsync(string location, string password)
        {
            _authenticator = await Authenticator.CreateAccountAsync(null, location, password);
            _secret = location;
            _password = password;
        }

        internal Task RevokeAppAsync(string appId)
        {
            return _authenticator.AuthRevokeAppAsync(appId);
        }

        ~AuthService()
        {
            FreeState();
        }

        public void FreeState()
        {
            if (_authenticator != null)
            {
                _authenticator.Dispose();
                _authenticator = null;
            }
        }

        internal async Task<List<RegisteredAppModel>> GetRegisteredAppsAsync()
        {
            var appList = await _authenticator.AuthRegisteredAppsAsync();
            foreach (var app in appList)
            {
                Utilities.UpdateAppContainerNameList(app.Id, app.Name);
            }
            return appList
                   .Select(app => new RegisteredAppModel(
                       new AppExchangeInfo()
                       {
                           Id = app.Id,
                           Name = app.Name,
                           Scope = string.Empty,
                           Vendor = app.Vendor,
                       },
                       app.Containers,
                       app.AppPermissions))
                   .ToList();
        }

        public async Task HandleUrlActivationAsync(string encodedUri)
        {
            try
            {
                if (await HandleUnregisteredAppRequest(encodedUri))
                    return;

                if (_authenticator == null)
                {
                    AuthenticationReq = encodedUri;
                    if (!AuthReconnect)
                    {
                        var response = await Application.Current.MainPage.DisplayAlert(
                            "Login Required",
                            "An application is requesting access, login to authorise",
                            "Login",
                            "Cancel");
                        if (response)
                        {
                            MessagingCenter.Send(this, MessengerConstants.NavPreviousPage);
                        }
                    }
                    return;
                }

                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    await CheckAndReconnect();
                }

                var encodedReq = UrlFormat.GetRequestData(encodedUri);
                var decodeResult = await _authenticator.DecodeIpcMessageAsync(encodedReq);
                var decodedType = decodeResult.GetType();
                if (decodedType == typeof(IpcReqError))
                {
                    var error = decodeResult as IpcReqError;
                    throw new FfiException(error.Code, error.Description);
                }
                else
                {
                    MessagingCenter.Send(this, MessengerConstants.NavPreviousPage);
                    var requestPage = new RequestDetailPage(encodedUri, decodeResult);
                    await Application.Current.MainPage.Navigation.PushPopupAsync(requestPage);
                }
            }
            catch (FfiException ex)
            {
                var errorMessage = Utilities.GetErrorMessage(ex);
                await Application.Current.MainPage.DisplayAlert("Authorisation Error", errorMessage, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        internal async Task<string> GetEncodedResponseAsync(IpcReq req, bool isGranted)
        {
            string encodedRsp = string.Empty;
            var requestType = req.GetType();
            if (requestType == typeof(UnregisteredIpcReq))
            {
                var uauthReq = req as UnregisteredIpcReq;
                encodedRsp = await Authenticator.AutheriseUnregisteredAppAsync(uauthReq.ReqId, isGranted);
            }
            else if (requestType == typeof(AuthIpcReq))
            {
                var authReq = req as AuthIpcReq;
                encodedRsp = await _authenticator.EncodeAuthRespAsync(authReq, isGranted);
            }
            else if (requestType == typeof(ContainersIpcReq))
            {
                var containerReq = req as ContainersIpcReq;
                encodedRsp = await _authenticator.EncodeContainersRespAsync(containerReq, isGranted);
            }
            else if (requestType == typeof(ShareMDataIpcReq))
            {
                var mDataShareReq = req as ShareMDataIpcReq;
                if (!isGranted)
                {
                    throw new Exception("SharedMData request denied");
                }
                encodedRsp = await _authenticator.EncodeShareMdataRespAsync(mDataShareReq, isGranted);
            }
            return encodedRsp;
        }

        private async Task<bool> HandleUnregisteredAppRequest(string encodedUri)
        {
            var encodedReq = UrlFormat.GetRequestData(encodedUri);
            var udecodeResult = await Authenticator.UnRegisteredDecodeIpcMsgAsync(encodedReq);
            if (udecodeResult.GetType() == typeof(UnregisteredIpcReq))
            {
                var requestPage = new RequestDetailPage(encodedUri, udecodeResult);
                await Application.Current.MainPage.Navigation.PushPopupAsync(requestPage);
                return true;
            }
            return false;
        }

        private async Task InitLoggingAsync()
        {
            await Authenticator.AuthInitLoggingAsync($"{DateTime.Now.ToShortTimeString()}.log");

            Debug.WriteLine("Rust Logging Initialised.");
            IsLogInitialised = true;
        }

        internal async Task LoginAsync(string location, string password)
        {
            _authenticator = await Authenticator.LoginAsync(location, password);
            _secret = location;
            _password = password;
        }

        internal async Task LogoutAsync()
        {
            await Task.Run(() =>
            {
                FreeState();
                if (AuthReconnect)
                {
                    CredentialCache.Delete();
                    AuthReconnect = false;
                }
            });
        }

        private void InternetConnectivityChanged(object obj, EventArgs args)
        {
            Device.BeginInvokeOnMainThread(
                async () =>
                {
                    if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                    {
                        return;
                    }
                    await CheckAndReconnect();
                });
        }

        private void OnNetworkDisconnected(object obj, EventArgs args)
        {
            Debug.WriteLine("Network Observer Fired");

            if (obj == null || _authenticator == null || obj as Authenticator != _authenticator)
            {
                return;
            }

            Device.BeginInvokeOnMainThread(
                async () =>
                {
                    if (App.IsBackgrounded)
                    {
                        return;
                    }

                    await CheckAndReconnect();
                });
        }

        private async void StoreCredentials()
        {
            await CredentialCache.Store(_secret, _password);
        }
    }
}
