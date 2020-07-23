// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System.Linq;
using System.Threading.Tasks;
using SafeAuthenticatorApp.Helpers;
using SafeAuthenticatorApp.Services;
using SafeAuthenticatorApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace SafeAuthenticatorApp
{
    public partial class App : Application
    {
        private static volatile bool _isBackgrounded;

        private AuthService Service => DependencyService.Get<AuthService>();

        internal static bool IsBackgrounded
        {
            get => _isBackgrounded;
            private set => _isBackgrounded = value;
        }

        public App()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<AuthService>(this, MessengerConstants.ResetAppViews, async _ => { await ResetViews(); });
            Current.MainPage = new NavigationPage(NewStartupPage());
            MessagingCenter.Subscribe<AuthService>(this, MessengerConstants.NavPreviousPage, async _ =>
            {
                var navigationStackSize = Current.MainPage.Navigation.NavigationStack.Count - 1;
                var topNavigationStackPageType = Current.MainPage.Navigation.NavigationStack[navigationStackSize].GetType();
                if (topNavigationStackPageType == typeof(SettingsPage) ||
                    topNavigationStackPageType == typeof(AppInfoPage) ||
                    topNavigationStackPageType == typeof(CreateAcctPage))
                {
                    await Current.MainPage.Navigation.PopAsync();
                }
            });
        }

        internal static bool IsPageValid(Page page)
        {
            if (!(Current.MainPage is NavigationPage navPage))
            {
                return false;
            }

            var validPage = navPage.Navigation.NavigationStack.FirstOrDefault();
            var checkPage = page.Navigation.NavigationStack.FirstOrDefault();
            return validPage != null && validPage == checkPage;
        }

        private Page NewStartupPage()
        {
            if (!Current.Properties.ContainsKey(Constants.IsTutorialComplete))
                return new TutorialPage();
            else
                return new LoginPage();
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await Service.CheckAndReconnect();
        }

        protected override async void OnResume()
        {
            base.OnResume();
            IsBackgrounded = false;
            await Service.CheckAndReconnect();
        }

        protected override async void OnSleep()
        {
            base.OnSleep();

            IsBackgrounded = true;
            await SavePropertiesAsync();
        }

        private async Task ResetViews()
        {
            if (!(Current.MainPage is NavigationPage navPage))
            {
                return;
            }

            var navigationController = navPage.Navigation;
            foreach (var page in navigationController.NavigationStack.OfType<ICleanup>())
            {
                page.MessageCenterUnsubscribe();
            }

            var rootPage = navigationController.NavigationStack.FirstOrDefault();
            if (rootPage == null)
            {
                return;
            }

            navigationController.InsertPageBefore(NewStartupPage(), rootPage);
            await navigationController.PopToRootAsync(true);
        }
    }
}
