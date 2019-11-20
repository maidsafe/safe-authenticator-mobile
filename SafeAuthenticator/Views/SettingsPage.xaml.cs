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

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

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
        }

        private void MessageCenterUnsubscribe()
        {
            MessagingCenter.Unsubscribe<HomeViewModel>(this, MessengerConstants.NavLoginPage);
        }
    }
}
