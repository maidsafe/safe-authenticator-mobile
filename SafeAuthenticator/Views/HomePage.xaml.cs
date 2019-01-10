using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views
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
