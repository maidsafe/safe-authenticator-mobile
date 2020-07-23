// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using SafeAuthenticatorApp.Helpers;
using SafeAuthenticatorApp.Models;
using SafeAuthenticatorApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticatorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage, ICleanup
    {
        private readonly HomeViewModel _homeViewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _homeViewModel.HandleAuthenticationReq();
        }

        public HomePage()
        {
            InitializeComponent();
            _homeViewModel = new HomeViewModel();
            BindingContext = _homeViewModel;

            MessagingCenter.Subscribe<HomeViewModel, RegisteredAppModel>(
                this,
                MessengerConstants.NavAppInfoPage,
                async (_, appInfo) =>
                {
                    if (!App.IsPageValid(this))
                    {
                        MessageCenterUnsubscribe();
                        return;
                    }
                    await Navigation.PushAsync(new AppInfoPage(appInfo));
                });

            MessagingCenter.Subscribe<HomeViewModel>(
                this,
                MessengerConstants.NavSettingsPage,
                async _ =>
                {
                    if (!App.IsPageValid(this))
                    {
                        MessageCenterUnsubscribe();
                        return;
                    }
                    await Navigation.PushAsync(new SettingsPage());
                });
        }

        public void MessageCenterUnsubscribe()
        {
            MessagingCenter.Unsubscribe<HomeViewModel>(this, MessengerConstants.NavSettingsPage);
            MessagingCenter.Unsubscribe<HomeViewModel, RegisteredAppModel>(this, MessengerConstants.NavAppInfoPage);
        }
    }
}
