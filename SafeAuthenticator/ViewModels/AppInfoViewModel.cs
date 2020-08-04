// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SafeApp.Core;
using SafeAuthenticatorApp.Helpers;
using SafeAuthenticatorApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeAuthenticatorApp.ViewModels
{
    internal class AppInfoViewModel : BaseViewModel
    {
        private RegisteredAppModel _appModelInfo;

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

        public ICommand RevokeAppCommand { get; private set; }

        public AppInfoViewModel(RegisteredAppModel appModelInfo)
        {
            AppModelInfo = appModelInfo;

            if (AppModelInfo.AppPermissions.GetBalance)
                TestCoinPermissions += "Check balance, ";

            if (AppModelInfo.AppPermissions.TransferCoins)
                TestCoinPermissions += "Transfer coins";

            if (!string.IsNullOrEmpty(TestCoinPermissions))
                ShowTestCoinPermissions = true;

            if (!string.IsNullOrEmpty(TestCoinPermissions) &&
                TestCoinPermissions.EndsWith(", ", StringComparison.OrdinalIgnoreCase))
                TestCoinPermissions = TestCoinPermissions.TrimEnd(',', ' ');

            RevokeAppCommand = new Command(async () => await OnRevokeAppCommand());
        }

        private async Task OnRevokeAppCommand()
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
