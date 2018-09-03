using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CommonUtils;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;
using SafeAuthenticator.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthService))]

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
            var (location, password) = CredentialCache.Retrieve();
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
      Debug.WriteLine($"CreateAccountAsync {location} - {password} - {invitation.Substring(0, 5)}");
      _authenticator = await Authenticator.CreateAccountAsync(location, password, invitation);
      if (AuthReconnect) {
        CredentialCache.Store(location, password);
      }
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
        var encodedReq = UrlFormat.GetRequestData(encodedUri);
        var decodeResult = await _authenticator.DecodeIpcMessageAsync(encodedReq);
        var decodedType = decodeResult.GetType();
        if (decodedType == typeof(AuthIpcReq)) {
          var authReq = decodeResult as AuthIpcReq;
          Debug.WriteLine($"Decoded Req From {authReq?.AuthReq.App.Name}");
          var isGranted = await Application.Current.MainPage.DisplayAlert(
            "Auth Request",
            $"{authReq?.AuthReq.App.Name} is requesting access",
            "Allow",
            "Deny");
          var encodedRsp = await _authenticator.EncodeAuthRespAsync(authReq, isGranted);
          var formattedRsp = UrlFormat.Format(authReq?.AuthReq.App.Id, encodedRsp, false);
          Debug.WriteLine($"Encoded Rsp to app: {formattedRsp}");
          Device.BeginInvokeOnMainThread(() => { Device.OpenUri(new Uri(formattedRsp)); });
        } else if (decodedType == typeof(IpcReqError)) {
          var error = decodeResult as IpcReqError;
          await Application.Current.MainPage.DisplayAlert("Auth Request", $"Error: {error?.Description}", "Ok");
        } else {
          Debug.WriteLine("Decoded Req is not Auth Req");
        }
      } catch (Exception ex) {
        var errorMsg = ex.Message;
        if (ex is ArgumentNullException) {
          errorMsg = "Ignoring Auth Request: Need to be logged in to accept app requests.";
        }

        await Application.Current.MainPage.DisplayAlert("Error", errorMsg, "OK");
      }
    }

    private async void InitLoggingAsync() {
      await Authenticator.AuthInitLoggingAsync(null);

      Debug.WriteLine("Rust Logging Initialised.");
      IsLogInitialised = true;
    }

    internal async Task LoginAsync(string location, string password) {
      Debug.WriteLine($"LoginAsync {location} - {password}");
      _authenticator = await Authenticator.LoginAsync(location, password);
      if (AuthReconnect) {
        CredentialCache.Store(location, password);
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
