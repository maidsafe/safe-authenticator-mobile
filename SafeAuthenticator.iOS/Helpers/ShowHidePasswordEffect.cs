// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System.Drawing;
using SafeAuthenticator.iOS.Helpers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Material.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(ShowHidePasswordEffect), "ShowHidePasswordEffect")]

namespace SafeAuthenticator.iOS.Helpers
{
    public class ShowHidePasswordEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            ConfigureControl();
        }

        protected override void OnDetached()
        {
        }

        private void ConfigureControl()
        {
            if (Control != null)
            {
                MaterialTextField vUpdatedEntry = (MaterialTextField)Control;
                var buttonRect = UIButton.FromType(UIButtonType.Custom);
                buttonRect.SetImage(new UIImage("ShowPasswordIcon"), UIControlState.Normal);
                buttonRect.ImageEdgeInsets = new UIEdgeInsets(10.0f, 10.0f, 10.0f, 10.0f);
                buttonRect.TouchUpInside += (sender, e1) =>
                {
                    if (vUpdatedEntry.SecureTextEntry)
                    {
                        vUpdatedEntry.SecureTextEntry = false;
                        buttonRect.SetImage(new UIImage("HidePasswordIcon"), UIControlState.Normal);
                    }
                    else
                    {
                        vUpdatedEntry.SecureTextEntry = true;
                        buttonRect.SetImage(new UIImage("ShowPasswordIcon"), UIControlState.Normal);
                    }
                };

                vUpdatedEntry.ShouldChangeCharacters += (textField, range, replacementString) =>
                {
                    string text = vUpdatedEntry.Text;
                    var result = text.Substring(0, (int)range.Location) +
                     replacementString + text.Substring((int)range.Location + (int)range.Length);
                    vUpdatedEntry.Text = result;
                    return false;
                };

                buttonRect.Frame = new CoreGraphics.CGRect(10.0f, 0.0f, 5.0f, 5.0f);
                buttonRect.ContentMode = UIViewContentMode.BottomRight;

                vUpdatedEntry.TrailingView = buttonRect;
                vUpdatedEntry.TrailingViewMode = UITextFieldViewMode.Always;
                vUpdatedEntry.TextAlignment = UITextAlignment.Left;
            }
        }
    }
}
