using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;
using SafeAuthenticator.Services;
using SafeAuthenticator.Views;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(AuthService))]

namespace SafeAuthenticator.Services {
  public class AuthService : ObservableObject, IDisposable {
    private const string AuthReconnectPropKey = nameof(AuthReconnect);
    private readonly SemaphoreSlim _reconnectSemaphore = new SemaphoreSlim(1, 1);
    private Authenticator _authenticator;
    private bool _isLogInitialised;
    internal bool IsLogInitialised { get => _isLogInitialised; private set => SetProperty(ref _isLogInitialised, value); }

    private CredentialCacheService CredentialCache { get; }

    internal bool AuthReconnect {
      get {
        if (!Application.Current.Properties.ContainsKey(AuthReconnectPropKey)) {
          return false;
        }

        var val = Application.Current.Properties[AuthReconnectPropKey] as bool?;
        return val == true;
      }
      set {
        if (value == false) {
          CredentialCache.Delete();
        }

        Application.Current.Properties[AuthReconnectPropKey] = value;
      }
    }

    public AuthService() {
      _isLogInitialised = false;
      CredentialCache = new CredentialCacheService();
      Authenticator.Disconnected += OnNetworkDisconnected;
      InitLoggingAsync();
    }

    public void Dispose() {
      // ReSharper disable once DelegateSubtraction
      Authenticator.Disconnected -= OnNetworkDisconnected;
      FreeState();
      GC.SuppressFinalize(this);
    }

    internal async Task CheckAndReconnect() {
      if (_authenticator == null) {
        return;
      }

      await _reconnectSemaphore.WaitAsync();
      try {
        if (_authenticator.IsDisconnected) {
          if (!AuthReconnect) {
            throw new Exception("Reconnect Disabled");
          }

          using (UserDialogs.Instance.Loading("Reconnecting to Network")) {
            var (location, password) = await CredentialCache.Retrieve();
            await LoginAsync(location, password);
          }

          try {
            var cts = new CancellationTokenSource(2000);
            await UserDialogs.Instance.AlertAsync("Network connection established.", "Success", "OK", cts.Token);
          } catch (OperationCanceledException) { }
        }
      } catch (Exception ex) {
        await Application.Current.MainPage.DisplayAlert("Error", $"Unable to Reconnect: {ex.Message}", "OK");
        FreeState();
        MessagingCenter.Send(this, MessengerConstants.ResetAppViews);
      } finally {
        _reconnectSemaphore.Release(1);
      }
    }

    internal async Task CreateAccountAsync(string location, string password, string invitation) {
      _authenticator = await Authenticator.CreateAccountAsync(location, password, invitation);
      if (AuthReconnect) {
        await CredentialCache.Store(location, password);
      }
    }
	  internal async Task<string> RevokeAppAsync(string appId) {
      return await _authenticator.AuthRevokeAppAsync(appId);
    }

    ~AuthService() {
      FreeState();
    }

    public void FreeState() {
      _authenticator?.Dispose();
    }

    internal async Task<(int, int)> GetAccountInfoAsync() {
      var acctInfo = await _authenticator.AuthAccountInfoAsync();
      return (Convert.ToInt32(acctInfo.MutationsDone), Convert.ToInt32(acctInfo.MutationsDone + acctInfo.MutationsAvailable));
    }

    internal async Task<List<RegisteredAppModel>> GetRegisteredAppsAsync() {
      var appList = await _authenticator.AuthRegisteredAppsAsync();
      return appList.Select(app => new RegisteredAppModel(app.AppInfo, app.Containers)).ToList();
    }

    public async Task HandleUrlActivationAsync(string encodedUri) {
      try {
        if (_authenticator == null) {
          return;
        }

        await CheckAndReconnect();
        if (encodedUri.Contains("/unregistered")) {
          var unregisteredRemoved = encodedUri.Replace("/unregistered", "");
          var uencodedReq = UrlFormat.GetRequestData(unregisteredRemoved);
          var udecodeResult = await Authenticator.UnRegisteredDecodeIpcMsgAsync(uencodedReq);
          var udecodedType = udecodeResult.GetType();
          if (udecodedType == typeof(UnregisteredIpcReq))
          {
            var uauthReq = udecodeResult as UnregisteredIpcReq;
            var isGranted = await Application.Current.MainPage.DisplayAlert(
                    "Unregistered auth request",
                    $"An app is requesting access.",
                    "Allow",
                    "Deny");
            var encodedRsp = await Authenticator.EncodeUnregisteredRespAsync(uauthReq.ReqId, isGranted);
            var appIdFromUrl = UrlFormat.GetAppId(encodedUri);
            var formattedRsp = UrlFormat.Format(appIdFromUrl, encodedRsp, false);
            Debug.WriteLine($"Encoded Rsp to app: {formattedRsp}");
            Device.BeginInvokeOnMainThread(() => { Device.OpenUri(new Uri(formattedRsp)); });
          }
        } else {
          var encodedReq = UrlFormat.GetRequestData(encodedUri);
          var decodeResult = await _authenticator.DecodeIpcMessageAsync(encodedReq);
          //var decodedType = decodeResult.GetType();
          //if (decodedType == typeof(AuthIpcReq)) {
          //      var authReq = decodeResult as AuthIpcReq;
          //      Debug.WriteLine($"Decoded Req From {authReq?.AuthReq.App.Name}");
                var requestPage = new RequestDetailPage(decodeResult);
                requestPage.AllowRequest += async (s,e) =>
                {
                    await SendResponseBack(decodeResult, true);
                };
                requestPage.DenyRequest +=  async (s, e) =>
                {
                    await SendResponseBack(decodeResult, false);
                };
                await Application.Current.MainPage.Navigation.PushModalAsync(requestPage);
           //} else if (decodedType == typeof(ContainersIpcReq)) {
           //     var containerReq = decodeResult as ContainersIpcReq;
           //     Debug.WriteLine($"Decoded Req From {containerReq?.ContainersReq.App.Name}");
           //     foreach (var item in containerReq.ContainersReq.Containers) {
           //         Debug.WriteLine($"Container Name {item.ContName}");
           //         Debug.WriteLine($"Container Permissions {item.Access.Insert}, {item.Access.Read},{item.Access.Update},{item.Access.Delete},{item.Access.ManagePermissions}");
           //     }
           //     var isGranted = await Application.Current.MainPage.DisplayAlert(
           //     "Container Request",
           //     $"{containerReq?.ContainersReq.App.Name} is requesting access",
           //     "Allow",
           //     "Deny");
                
           // } else if (decodedType == typeof(ShareMDataReq)) {
           //     var shareMdReq = decodeResult as ShareMDataIpcReq;
           //     Debug.WriteLine($"Decoded Req From {shareMdReq?.ShareMDataReq.App.Name}");
           //     foreach (var item in shareMdReq.ShareMDataReq.MData) {
           //         Debug.WriteLine($"Container Name {item.Name}, {item.TypeTag}");
           //         Debug.WriteLine($"Container Permissions {item.Perms.Insert}, {item.Perms.Read},{item.Perms.Update},{item.Perms.Delete},{item.Perms.ManagePermissions}");
           //     }
           //     var isGranted = await Application.Current.MainPage.DisplayAlert(
           //     "Mutable Data Share Request",
           //     $"{shareMdReq?.ShareMDataReq.App.Name} is requesting access",
           //     "Allow",
           //     "Deny");
                
             
        //else if (decodedType == typeof(IpcReqError)) {
        //      var error = decodeResult as IpcReqError;
        //      await Application.Current.MainPage.DisplayAlert("Auth Request", $"Error: {error?.Description}", "Ok");
        //  } else {
        //      Debug.WriteLine("Not a valid request");
        //    }
          }
        } catch (Exception ex) {
        var errorMsg = ex.Message;
        if (ex is ArgumentNullException) {
          errorMsg = "Ignoring Auth Request: Need to be logged in to accept app requests.";
        }

        await Application.Current.MainPage.DisplayAlert("Error", errorMsg, "OK");
      }
    }

    private async Task SendResponseBack(IpcReq req, bool isGranted) {
            var encodedRsp = string.Empty;
            var formattedRsp = string.Empty;
            var requestType = req.GetType();
            if(requestType == typeof(AuthIpcReq))
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

    private async void InitLoggingAsync() {
      await Authenticator.AuthInitLoggingAsync(null);

      Debug.WriteLine("Rust Logging Initialised.");
      IsLogInitialised = true;
    }

    internal async Task LoginAsync(string location, string password) {
      _authenticator = await Authenticator.LoginAsync(location, password);
      if (AuthReconnect) {
        await CredentialCache.Store(location, password);
      }
    }

    internal async Task LogoutAsync() {
      await Task.Run(() => { _authenticator.Dispose(); });
    }

    private void OnNetworkDisconnected(object obj, EventArgs args) {
      Debug.WriteLine("Network Observer Fired");

      if (obj == null || _authenticator == null || obj as Authenticator != _authenticator) {
        return;
      }

      Device.BeginInvokeOnMainThread(
        async () => {
          if (App.IsBackgrounded) {
            return;
          }

          await CheckAndReconnect();
        });
    }
  }
}
