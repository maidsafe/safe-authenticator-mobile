using System.Diagnostics;
using System.Threading.Tasks;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Services;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage, ICleanup
    {
        public LoginPage()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<AuthService>(
                this,
                MessengerConstants.NavHomePage,
                async sender =>
                {
                    MessageCenterUnsubscribe();
                    if (!App.IsPageValid(this))
                    {
                        return;
                    }

                    Navigation.InsertPageBefore(new HomePage(), this);
                    await Navigation.PopAsync();
                });

            MessagingCenter.Subscribe<LoginViewModel>(
                this,
                MessengerConstants.NavHomePage,
                async sender =>
                {
                    MessageCenterUnsubscribe();
                    if (!App.IsPageValid(this))
                    {
                        return;
                    }

                    Navigation.InsertPageBefore(new HomePage(), this);
                    await Navigation.PopAsync();
                });

            MessagingCenter.Subscribe<LoginViewModel>(
                this,
                MessengerConstants.NavCreateAcctPage,
                async _ =>
                {
                    if (!App.IsPageValid(this))
                    {
                        MessageCenterUnsubscribe();
                        return;
                    }

                    NavigationPage.SetBackButtonTitle(this, "Login");
                    await Navigation.PushAsync(new CreateAcctPage());
                });

            var connectionMenuTapGestureRecogniser = new TapGestureRecognizer()
            {
                NumberOfTapsRequired = 1
            };
            connectionMenuTapGestureRecogniser.Tapped += (s, e) =>
            {
                Navigation.PushAsync(new VaultConnectionFilePage());
            };
            ConnectionSettingsMenuIcon.GestureRecognizers.Add(connectionMenuTapGestureRecogniser);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext != null)
            {
                var loginPageViewModel = (LoginViewModel)BindingContext;

                if (loginPageViewModel != null)
                {
                    if (loginPageViewModel.VaultConnectionFileExists())
                    {
                        await loginPageViewModel.SetVaultConnectionConfigFileDirAsync();
                    }
                    else
                    {
                        var result = await DisplayAlert(
                            "No vault connection file found",
                            "Please choose a vault for connection from the settings menu.",
                            "Settings",
                            "Cancel");

                        if (result)
                        {
                            await Navigation.PushAsync(new VaultConnectionFilePage());
                        }
                    }
                }
            }
        }

        public void MessageCenterUnsubscribe()
        {
            MessagingCenter.Unsubscribe<AuthService>(this, MessengerConstants.NavHomePage);
            MessagingCenter.Unsubscribe<LoginViewModel>(this, MessengerConstants.NavHomePage);
            MessagingCenter.Unsubscribe<LoginViewModel>(this, MessengerConstants.NavCreateAcctPage);
        }

        private void CustomEntryFocused(object sender, FocusEventArgs e)
        {
            ScrollLayout.IsScrollEnabled = true;
        }

        private void CustomEntryUnfocused(object sender, FocusEventArgs e)
        {
            ScrollLayout.IsScrollEnabled = false;
        }
    }
}
