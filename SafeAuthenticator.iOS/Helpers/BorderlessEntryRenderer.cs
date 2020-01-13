// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System.Drawing;
using CoreAnimation;
using CoreGraphics;
using SafeAuthenticator.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Color = Xamarin.Forms.Color;

// [assembly: ExportRenderer(typeof(MaterialEntry), typeof(BorderlessEntryRenderer))]
namespace SafeAuthenticator.iOS.Helpers
{
    public class BorderlessEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || e.NewElement == null)
                return;

            var buttonRect = UIButton.FromType(UIButtonType.Custom);
            buttonRect.Frame = new RectangleF(0, 0, 30, 30);
            buttonRect.SetImage(new UIImage("ShowPasswordIcon"), UIControlState.Normal);
            buttonRect.TouchUpInside += (sender, e1) =>
            {
                if (Control.SecureTextEntry)
                {
                    Control.SecureTextEntry = false;
                    buttonRect.SetImage(new UIImage("HidePasswordIcon"), UIControlState.Normal);
                }
                else
                {
                    Control.SecureTextEntry = true;
                    buttonRect.SetImage(new UIImage("ShowPasswordIcon"), UIControlState.Normal);
                }
            };

            Control.ShouldChangeCharacters += (textField, range, replacementString) =>
            {
                var text = Control.Text;
                var result = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
                Control.Text = result;
                return false;
            };

            Control.BorderStyle = UITextBorderStyle.None;
            var borderLayer = new CALayer();

            if (((MaterialEntry)Element).IsPassword)
            {
                Control.RightViewMode = UITextFieldViewMode.Always;
                Control.RightView = buttonRect;
                borderLayer.Frame = new CGRect(
                    0f,
                    Element.HeightRequest - 1,
                    Frame.Width + Control.RightView.Frame.Width + 10,
                    1f);
                borderLayer.BorderColor = Color.FromHex("#0074e4").ToCGColor();
                borderLayer.BorderWidth = 3.0f;
            }
            else
            {
                borderLayer.Frame = new CGRect(0f, Element.HeightRequest - 1, Frame.Width, 1f);
                borderLayer.BorderColor = Color.FromHex("#0074e4").ToCGColor();
                borderLayer.BorderWidth = 2.0f;
            }

            Control.Layer.AddSublayer(borderLayer);
            Control.Layer.MasksToBounds = true;
        }
    }
}
