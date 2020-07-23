﻿// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using SafeAuthenticatorApp.Helpers;
using SafeAuthenticatorApp.Models;
using SafeAuthenticatorApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticatorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    // ReSharper disable once MemberCanBeInternal
    public partial class AppInfoPage : ContentPage, ICleanup
    {
        // ReSharper disable once MemberCanBeInternal
        public AppInfoPage()
            : this(null)
        {
        }

        // ReSharper disable once MemberCanBeInternal
        public AppInfoPage(RegisteredAppModel appModelInfo)
        {
            InitializeComponent();
            BindingContext = new AppInfoViewModel(appModelInfo);

            MessagingCenter.Subscribe<AppInfoViewModel>(
                this,
                MessengerConstants.NavHomePage,
                async _ =>
                {
                    MessageCenterUnsubscribe();
                    if (!App.IsPageValid(this))
                    {
                        return;
                    }

                    await Navigation.PopAsync();
                });
        }

        public void MessageCenterUnsubscribe()
        {
            MessagingCenter.Unsubscribe<AppInfoViewModel>(this, MessengerConstants.NavHomePage);
        }
    }
}
