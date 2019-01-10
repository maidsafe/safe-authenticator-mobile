using SafeAuthenticator.Helpers;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private readonly SettingsViewModel _settingsViewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _settingsViewModel.GetAccountInfo();
        }

        public SettingsPage()
        {
            InitializeComponent();
            _settingsViewModel = new SettingsViewModel();
            BindingContext = _settingsViewModel;

            AccountStatusImage.Clicked += (s, e) =>
            {
                DisplayAlert("Account Status", "The number of store and modify operations completed on this account.", "ok");
            };

            MessagingCenter.Subscribe<SettingsViewModel>(
                this,
                MessengerConstants.NavLoginPage,
                async _ =>
                {
                    MessageCenterUnsubscribe();
                    if (!App.IsPageValid(this))
                    {
                        return;
                    }
                    Navigation.InsertPageBefore(new LoginPage(), Navigation.NavigationStack[0]);
                    await Navigation.PopToRootAsync();
                });
        }

        private void MessageCenterUnsubscribe()
        {
            MessagingCenter.Unsubscribe<HomeViewModel>(this, MessengerConstants.NavLoginPage);
        }
    }
}
