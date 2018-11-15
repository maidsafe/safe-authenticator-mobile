using SafeAuthenticator.Native;
using SafeAuthenticator.ViewModels;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RequestDetailPage : ContentPage
    {
        public event EventHandler AllowRequest;

        public event EventHandler DenyRequest;

        RequestDetailViewModel _viewModel;

        public RequestDetailPage(IpcReq req)
        {
            InitializeComponent();
            _viewModel = new RequestDetailViewModel(req);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = _viewModel;
        }

        private async Task Allow_Request(object sender, EventArgs e)
        {
            AllowRequest?.Invoke(this, EventArgs.Empty);
            await Navigation.PopModalAsync();
        }

        private async Task Deny_Request(object sender, EventArgs e)
        {
            DenyRequest?.Invoke(this, EventArgs.Empty);
            await Navigation.PopModalAsync();
        }
    }
}