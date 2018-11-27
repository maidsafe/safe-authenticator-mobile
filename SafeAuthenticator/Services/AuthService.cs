using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;
using SafeAuthenticator.Services;
using SafeAuthenticator.Views;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthService))]

namespace SafeAuthenticator.Services
{
    public class AuthService : ObservableObject, IDisposable
    {
        private const string AuthReconnectPropKey = nameof(AuthReconnect);
        private readonly SemaphoreSlim _reconnectSemaphore = new SemaphoreSlim(1, 1);
        private Authenticator _authenticator;
        private bool _isLogInitialised;

        public string AuthenticationReq { get; set; }

        internal bool IsLogInitialised
        {
            get => _isLogInitialised;
            private set => SetProperty(ref _isLogInitialised, value);
        }

        private CredentialCacheService CredentialCache { get; }

        internal bool AuthReconnect
        {
            get
            {
                if (!Application.Current.Properties.ContainsKey(AuthReconnectPropKey))
                {
                    return false;
                }

                var val = Application.Current.Properties[AuthReconnectPropKey] as bool?;
                return val == true;
            }

            set
            {
                if (value == false)
                {
                    CredentialCache.Delete();
                }

                Application.Current.Properties[AuthReconnectPropKey] = value;
            }
        }

        public AuthService()
        {
            _isLogInitialised = false;
            CredentialCache = new CredentialCacheService();
            Authenticator.Disconnected += OnNetworkDisconnected;
            Task.Run(async () => await InitLoggingAsync());
        }

        public void Dispose()
        {
            // ReSharper disable once DelegateSubtraction
            Authenticator.Disconnected -= OnNetworkDisconnected;
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
                    var (location, password) = await CredentialCache.Retrieve();
                    using (UserDialogs.Instance.Loading("Reconnecting to Network"))
                    {
                        await LoginAsync(location, password);
                        MessagingCenter.Send(this, MessengerConstants.NavHomePage);
                    }
                    return;
                }

                if (_authenticator.IsDisconnected)
                {
                    if (!AuthReconnect)
                    {
                        throw new Exception("Reconnect Disabled");
                    }

                    using (UserDialogs.Instance.Loading("Reconnecting to Network"))
                    {
                        var (location, password) = await CredentialCache.Retrieve();
                        await LoginAsync(location, password);
                    }
                }
            }
            catch (NullReferenceException)
            {
                // ignore if secret/password is not cached
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

        internal async Task CreateAccountAsync(string location, string password, string invitation)
        {
            _authenticator = await Authenticator.CreateAccountAsync(location, password, invitation);
            if (AuthReconnect)
            {
                await CredentialCache.Store(location, password);
            }
        }

        internal async Task<string> RevokeAppAsync(string appId)
        {
            return await _authenticator.AuthRevokeAppAsync(appId);
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

        internal async Task<(int, int)> GetAccountInfoAsync()
        {
            var acctInfo = await _authenticator.AuthAccountInfoAsync();
            return (Convert.ToInt32(acctInfo.MutationsDone),
                Convert.ToInt32(acctInfo.MutationsDone + acctInfo.MutationsAvailable));
        }

        internal async Task<List<RegisteredAppModel>> GetRegisteredAppsAsync()
        {
            var appList = await _authenticator.AuthRegisteredAppsAsync();
            return appList.Select(app => new RegisteredAppModel(app.AppInfo, app.Containers)).ToList();
        }

        public async Task HandleUrlActivationAsync(string encodedUri)
        {
            try
            {
                if (_authenticator == null)
                {
                    AuthenticationReq = encodedUri;
                    try
                    {
                        var (location, password) = await CredentialCache.Retrieve();
                    }
                    catch (NullReferenceException)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Need to be logged in to accept app requests", "OK");
                    }

                    return;
                }

                await CheckAndReconnect();
                if (encodedUri.Contains("/unregistered"))
                {
                    var unregisteredRemoved = encodedUri.Replace("/unregistered", string.Empty);
                    var uencodedReq = UrlFormat.GetRequestData(unregisteredRemoved);
                    var udecodeResult = await Authenticator.UnRegisteredDecodeIpcMsgAsync(uencodedReq);
                    var udecodedType = udecodeResult.GetType();
                    if (udecodedType == typeof(UnregisteredIpcReq))
                    {
                        var uauthReq = udecodeResult as UnregisteredIpcReq;
                        var isGranted = await Application.Current.MainPage.DisplayAlert(
                            "Unregistered auth request",
                            "An app is requesting access.",
                            "Allow",
                            "Deny");
                        var encodedRsp = await Authenticator.EncodeUnregisteredRespAsync(uauthReq.ReqId, isGranted);
                        var appIdFromUrl = UrlFormat.GetAppId(encodedUri);
                        var formattedRsp = UrlFormat.Format(appIdFromUrl, encodedRsp, false);
                        Debug.WriteLine($"Encoded Rsp to app: {formattedRsp}");
                        Device.BeginInvokeOnMainThread(() => { Device.OpenUri(new Uri(formattedRsp)); });
                    }
                }
                else
                {
                    var encodedReq = UrlFormat.GetRequestData(encodedUri);
                    var decodeResult = await _authenticator.DecodeIpcMessageAsync(encodedReq);
                    var decodedType = decodeResult.GetType();
                    if (decodedType == typeof(IpcReqError))
                    {
                        var error = decodeResult as IpcReqError;
                        await Application.Current.MainPage.DisplayAlert("Auth Request", $"Error: {error?.Description}", "Ok");
                    }
                    else
                    {
                        var requestPage = new RequestDetailPage(decodeResult);
                        requestPage.CompleteRequest += async (s, e) =>
                        {
                            var args = e as ResponseEventArgs;
                            await SendResponseBack(decodeResult, args.Response);
                        };
                        await Application.Current.MainPage.Navigation.PushModalAsync(requestPage);
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message;
                if (ex is ArgumentNullException)
                {
                    errorMsg = "Ignoring Auth Request: Need to be logged in to accept app requests.";
                }

                await Application.Current.MainPage.DisplayAlert("Error", errorMsg, "OK");
            }
        }

        private async Task SendResponseBack(IpcReq req, bool isGranted)
        {
            string encodedRsp;
            var formattedRsp = string.Empty;
            var requestType = req.GetType();
            if (requestType == typeof(AuthIpcReq))
            {
                var authReq = req as AuthIpcReq;
                encodedRsp = await _authenticator.EncodeAuthRespAsync(authReq, isGranted);
                formattedRsp = UrlFormat.Format(authReq?.AuthReq.App.Id, encodedRsp, false);
            }
            else if (requestType == typeof(ContainersIpcReq))
            {
                var containerReq = req as ContainersIpcReq;
                encodedRsp = await _authenticator.EncodeContainersRespAsync(containerReq, isGranted);
                formattedRsp = UrlFormat.Format(containerReq?.ContainersReq.App.Id, encodedRsp, false);
            }
            else if (requestType == typeof(ShareMDataIpcReq))
            {
                var mDataShareReq = req as ShareMDataIpcReq;
                encodedRsp = await _authenticator.EncodeShareMdataRespAsync(mDataShareReq, isGranted);
                formattedRsp = UrlFormat.Format(mDataShareReq?.ShareMDataReq.App.Id, encodedRsp, false);
            }

            Debug.WriteLine($"Encoded Rsp to app: {formattedRsp}");
            Device.BeginInvokeOnMainThread(() => { Device.OpenUri(new Uri(formattedRsp)); });
        }

        private async Task InitLoggingAsync()
        {
            await Authenticator.AuthInitLoggingAsync(null);

            Debug.WriteLine("Rust Logging Initialised.");
            IsLogInitialised = true;
        }

        internal async Task LoginAsync(string location, string password)
        {
            _authenticator = await Authenticator.LoginAsync(location, password);
            if (AuthReconnect)
            {
                await CredentialCache.Store(location, password);
            }
        }

        internal async Task LogoutAsync()
        {
            await Task.Run(() =>
            {
                FreeState();
                CredentialCache.Delete();
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
    }
}
