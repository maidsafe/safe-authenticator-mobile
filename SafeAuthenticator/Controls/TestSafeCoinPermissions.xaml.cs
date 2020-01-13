// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Controls
{
    public partial class TestSafeCoinPermissions : ContentView
    {
        public string PermissionsText
        {
            get { return (string)GetValue(PermissionsTextProperty); }
            set { SetValue(PermissionsTextProperty, value); }
        }

        public static readonly BindableProperty PermissionsTextProperty =
            BindableProperty.Create(
                nameof(PermissionsText),
                typeof(string),
                typeof(TestSafeCoinPermissions),
                string.Empty,
                propertyChanged: TestCoinsPermissionsTextPropertyChanged);

        private static void TestCoinsPermissionsTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue == null)
                return;

            var control = (TestSafeCoinPermissions)bindable;
            control.TestCoinsPermissionText.Text = newValue.ToString();
        }

        public TestSafeCoinPermissions()
        {
            InitializeComponent();
        }
    }
}
