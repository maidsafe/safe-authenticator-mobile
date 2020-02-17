// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Diagnostics;
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

        void DeleteFileContextClicked(object sender, System.EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.VaultConnectionFileSelectionCommand.Execute((sender as MenuItem).CommandParameter);
            }
        }

        void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                if (sender is ListView listView && listView.SelectedItem != null)
                    listView.SelectedItem = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
