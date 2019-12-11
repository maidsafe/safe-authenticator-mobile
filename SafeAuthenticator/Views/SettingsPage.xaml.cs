using System;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private readonly SettingsViewModel _settingsViewModel;

        public SettingsPage()
        {
            InitializeComponent();
            _settingsViewModel = new SettingsViewModel();
            BindingContext = _settingsViewModel;

            if (Device.RuntimePlatform == Device.Android &&
                 DeviceInfo.Version < Version.Parse("4.4"))
            {
                AutoReconnectLayout.IsVisible = false;
            }

            var tap = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };

            tap.Tapped += (s, e) =>
            {
                DisplayAlert("Account Status", "The number of store and modify operations completed on this account.", "OK");
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

            MessagingCenter.Subscribe<SettingsViewModel>(
                this,
                MessengerConstants.NavVaultConnectionManagerPage,
                _ =>
                {
                    MessageCenterUnsubscribe();
                    if (!App.IsPageValid(this))
                    {
                        return;
                    }

                    DisplayAlert(
                        "Choose a vault",
                        "Please logout and choose a different vault to connect from the settings on the login page.",
                        "ok");
                });
        }

        private void MessageCenterUnsubscribe()
        {
            MessagingCenter.Unsubscribe<HomeViewModel>(this, MessengerConstants.NavLoginPage);
            MessagingCenter.Unsubscribe<HomeViewModel>(this, MessengerConstants.NavVaultConnectionManagerPage);
        }
    }
}
