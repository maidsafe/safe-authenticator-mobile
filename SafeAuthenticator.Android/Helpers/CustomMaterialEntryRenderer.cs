// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System.ComponentModel;
using Android.Content;
using Android.Text;
using Android.Text.Method;
using SafeAuthenticator.Droid.Helpers;
using SafeAuthenticatorApp.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Material.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(
    typeof(CustomMaterialEntry),
    typeof(CustomMaterialEntryRenderer),
    new[] { typeof(VisualMarker.MaterialVisual) })]

namespace SafeAuthenticator.Droid.Helpers
{
    public class CustomMaterialEntryRenderer : MaterialEntryRenderer
    {
        public CustomMaterialEntryRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                SetInputType();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Entry.IsPasswordProperty.PropertyName ||
                e.PropertyName == InputView.KeyboardProperty.PropertyName)
                SetInputType();
        }

        private void SetInputType()
        {
            EditText.InputType = Element.Keyboard.ToInputType();

            if (Element.IsPassword && (EditText.InputType & InputTypes.ClassText) == InputTypes.ClassText)
            {
                EditText.TransformationMethod = new PasswordTransformationMethod();
                EditText.InputType = EditText.InputType | InputTypes.TextVariationPassword;
            }

            if (Element.IsPassword && (EditText.InputType & InputTypes.ClassNumber) == InputTypes.ClassNumber)
            {
                EditText.TransformationMethod = new PasswordTransformationMethod();
                EditText.InputType = EditText.InputType | InputTypes.NumberVariationPassword;
            }

            if (Element.IsPassword)
            {
                Control.PasswordVisibilityToggleEnabled = true;
            }
        }
    }
}
