using System;
using System.Windows.Input;
using JetBrains.Annotations;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;
using Xamarin.Essentials;
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

        private bool _showTestCoinPermissions;

        public bool ShowTestCoinPermissions
        {
            get => _showTestCoinPermissions;
            set => SetProperty(ref _showTestCoinPermissions, value);
        }

        private string _testCoinPermissions;

        public string TestCoinPermissions
        {
            get => _testCoinPermissions;
            set => SetProperty(ref _testCoinPermissions, value);
        }

        [PublicAPI]
        public ICommand RevokeAppCommand { get; }

        public AppInfoViewModel(RegisteredAppModel appModelInfo)
        {
            AppModelInfo = appModelInfo;

            if (AppModelInfo.AppPermissions.GetBalance)
                TestCoinPermissions += "Check balance ,";

            if (AppModelInfo.AppPermissions.TransferCoins)
                TestCoinPermissions += "Transfer coins";

            if (!string.IsNullOrEmpty(TestCoinPermissions))
                ShowTestCoinPermissions = true;
            else
                return;

            if (TestCoinPermissions.EndsWith(",", StringComparison.OrdinalIgnoreCase))
                TestCoinPermissions = TestCoinPermissions.TrimEnd(',');

            RevokeAppCommand = new Command(OnRevokeAppCommand);
        }

        private async void OnRevokeAppCommand()
        {
            if (await Application.Current.MainPage.DisplayAlert(
                "Revoke application",
                $"Are you sure you want to revoke access for {_appModelInfo.AppName}?",
                "Revoke",
                "Cancel"))
            {
                try
                {
                    if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                    {
                        throw new Exception(Constants.NoInternetMessage);
                    }
                    using (NativeProgressDialog.ShowNativeDialog("Revoking application"))
                    {
                        Authenticator.IsRevocationComplete = false;
                        await Authenticator.RevokeAppAsync(_appModelInfo.AppId);
                        Authenticator.IsRevocationComplete = true;
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
                    await Application.Current.MainPage.DisplayAlert("Error", $"Revoke Application Failed: {ex.Message}", "OK");
                }
            }
        }
    }
}
