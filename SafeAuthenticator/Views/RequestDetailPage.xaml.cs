using System;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RequestDetailPage : ContentPage
    {
        public event EventHandler CompleteRequest;

        private readonly RequestDetailViewModel _viewModel;

        public RequestDetailPage(IpcReq req)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                Padding = new Thickness(0, 20, 0, 0);
            }

            InitializeComponent();
            _viewModel = new RequestDetailViewModel(req);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = _viewModel;
        }

        private async void Send_Response(object sender, EventArgs e)
        {
            if (sender == AllowButton)
            {
                CompleteRequest?.Invoke(this, new ResponseEventArgs(true));
            }
            else if (sender == DenyButton)
            {
                CompleteRequest?.Invoke(this, new ResponseEventArgs(false));
            }

            await Navigation.PopModalAsync();
        }

        private void Unselect_Item(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
            {
                return;
            }

            if (sender is ListView lv)
            {
                lv.SelectedItem = null;
            }
        }
    }
}
