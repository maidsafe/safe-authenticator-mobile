using System;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RequestDetailPage : PopupPage
    {
        public event EventHandler CompleteRequest;

        private readonly RequestDetailViewModel _viewModel;

        public RequestDetailPage(IpcReq req)
        {
            InitializeComponent();

            InfoIcon.Clicked += (s, e) =>
            {
                AppDetailsStackLayout.IsVisible = !AppDetailsStackLayout.IsVisible;
                if (AppDetailsStackLayout.IsVisible)
                    PopupLayout.HeightRequest += 85;
                else
                    PopupLayout.HeightRequest -= 85;
            };

            _viewModel = new RequestDetailViewModel(req);
            BindingContext = _viewModel;
        }

        private void Send_Response(object sender, EventArgs e)
        {
            if (sender == AllowButton)
            {
                CompleteRequest?.Invoke(this, new ResponseEventArgs(true));
            }
            else if (sender == DenyButton)
            {
                CompleteRequest?.Invoke(this, new ResponseEventArgs(false));
            }
            PopupNavigation.Instance.PopAsync();
        }
    }
}
