// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System.Linq;
using SafeAuthenticatorApp.Helpers;
using SafeAuthenticatorApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticatorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateAcctPage : ContentPage, ICleanup
    {
        public CreateAcctPage()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<CreateAcctViewModel>(
                this,
                MessengerConstants.NavHomePage,
                async _ =>
                {
                    MessageCenterUnsubscribe();
                    if (!App.IsPageValid(this))
                    {
                        return;
                    }

                    var rootPage = Navigation.NavigationStack.FirstOrDefault();
                    if (rootPage == null)
                    {
                        return;
                    }
                    Navigation.InsertPageBefore(new HomePage(), rootPage);
                    await Navigation.PopToRootAsync();
                });
        }

        public void MessageCenterUnsubscribe()
        {
            MessagingCenter.Unsubscribe<CreateAcctViewModel>(this, MessengerConstants.NavHomePage);
        }
    }
}
