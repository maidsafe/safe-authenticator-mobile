// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using Rg.Plugins.Popup.Pages;
using SafeAuthenticator;
using SafeAuthenticatorApp.ViewModels;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticatorApp.Views
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
