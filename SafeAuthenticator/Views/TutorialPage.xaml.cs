// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using SafeAuthenticatorApp.Helpers;
using SafeAuthenticatorApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticatorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TutorialPage : ContentPage
    {
        public TutorialPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<TutorialViewModel>(
                this,
                MessengerConstants.NavCreateAcctPage,
                async _ =>
                {
                    if (!App.IsPageValid(this))
                    {
                        MessageCenterUnsubscribe();
                        return;
                    }
                    await Navigation.PushAsync(new CreateAcctPage());
                    Navigation.InsertPageBefore(new LoginPage(),  Navigation.NavigationStack[1]);
                    Navigation.RemovePage(Navigation.NavigationStack[0]);
                });

            MessagingCenter.Subscribe<TutorialViewModel>(
                this,
                MessengerConstants.NavLoginPage,
                async _ =>
                {
                    if (!App.IsPageValid(this))
                    {
                        MessageCenterUnsubscribe();
                        return;
                    }
                    Navigation.InsertPageBefore(new LoginPage(), this);
                    await Navigation.PopAsync();
                });
        }

        private void MessageCenterUnsubscribe()
        {
            MessagingCenter.Unsubscribe<TutorialViewModel>(this, MessengerConstants.NavCreateAcctPage);
            MessagingCenter.Unsubscribe<TutorialViewModel>(this, MessengerConstants.NavLoginPage);
        }
    }
}
