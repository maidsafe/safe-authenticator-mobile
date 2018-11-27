using System;
using System.Windows.Input;
using Acr.UserDialogs;
using JetBrains.Annotations;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels
{
    internal class AppInfoViewModel : BaseViewModel
    {
        private RegisteredAppModel _appModelInfo;

        [PublicAPI]
        public RegisteredAppModel AppModelInfo
        {
            get => _appModelInfo;
            set => SetProperty(ref _appModelInfo, value);
        }

        [PublicAPI]
        public ICommand RevokeAppCommand { get; }

        public AppInfoViewModel(RegisteredAppModel appModelInfo)
        {
            AppModelInfo = appModelInfo;
            RevokeAppCommand = new Command(OnRevokeAppCommand);
        }

        private async void OnRevokeAppCommand()
        {
            try
            {
                using (UserDialogs.Instance.Loading("Revoking permission"))
                {
                    await Authenticator.RevokeAppAsync(_appModelInfo.AppId);
                    MessagingCenter.Send(this, MessengerConstants.NavHomePage);
                    MessagingCenter.Send(this, MessengerConstants.RefreshHomePage);
                }
            }
            catch (FfiException ex)
            {
                var errorMessage = Utilities.GetErrorMessage(ex);
                await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Revoke app Failed: {ex.Message}", "OK");
            }
        }
    }
}
