using System.Linq;
using System.Threading.Tasks;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Services;
using SafeAuthenticator.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SafeAuthenticator
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
            MessagingCenter.Subscribe<AuthService>(this, MessengerConstants.NavHomePage, async _ =>
            {
                var navigationStackSize = Current.MainPage.Navigation.NavigationStack.Count - 1;
                var topNavigationStackPageType = Current.MainPage.Navigation.NavigationStack[navigationStackSize].GetType();
                if (topNavigationStackPageType == typeof(SettingsPage) || topNavigationStackPageType == typeof(AppInfoPage))
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
            if (!Current.Properties.ContainsKey(Constants.IsFirstLaunch))
            {
                Current.Properties[Constants.IsFirstLaunch] = true;
                return new TutorialPage();
            }
            else
            {
                return new LoginPage();
            }
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
