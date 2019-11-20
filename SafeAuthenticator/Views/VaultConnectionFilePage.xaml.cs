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
