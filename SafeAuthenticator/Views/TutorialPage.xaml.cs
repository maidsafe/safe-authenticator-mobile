using SafeAuthenticator.Helpers;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views
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
