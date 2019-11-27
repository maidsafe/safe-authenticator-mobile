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
