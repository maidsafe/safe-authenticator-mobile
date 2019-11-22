using SafeAuthenticator.Helpers;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VaultConnectionFilePage : ContentPage
    {
        private VaultConnectionFileViewModel _viewModel;

        public VaultConnectionFilePage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<VaultConnectionFileViewModel>(
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
            MessagingCenter.Unsubscribe<VaultConnectionFileViewModel>(this, MessengerConstants.NavLoginPage);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
            {
                _viewModel = new VaultConnectionFileViewModel();
                BindingContext = _viewModel;
            }
        }
    }
}
