using Rg.Plugins.Popup.Pages;
using SafeAuthenticator.Native;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RequestDetailPage : PopupPage
    {
        private readonly RequestDetailViewModel _viewModel;

        public RequestDetailPage(string encodedUri, IpcReq req)
        {
            InitializeComponent();

            _viewModel = new RequestDetailViewModel(encodedUri, req);
            BindingContext = _viewModel;
        }
    }
}
