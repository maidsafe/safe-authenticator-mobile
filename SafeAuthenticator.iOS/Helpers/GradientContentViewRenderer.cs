// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using CoreAnimation;
using CoreGraphics;
using SafeAuthenticator.Controls;
using SafeAuthenticator.iOS.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(GradientContentView), typeof(GradientContentViewRenderer))]

namespace SafeAuthenticator.iOS.Helpers
{
    public class GradientContentViewRenderer : VisualElementRenderer<ContentView>
    {
        private GradientContentView GradientContentView => (GradientContentView)Element;

        protected CAGradientLayer GradientLayer { get; set; }

        protected override void OnElementChanged(ElementChangedEventArgs<ContentView> e)
        {
            base.OnElementChanged(e);

            if (GradientContentView != null && NativeView != null)
            {
                GradientLayer = new CAGradientLayer();
                GradientLayer.Frame = NativeView.Bounds;
                GradientLayer.Colors = new CGColor[]
                {
                    GradientContentView.StartColor.ToCGColor(),
                    GradientContentView.EndColor.ToCGColor()
                };

                SetOrientation();

                NativeView.Layer.InsertSublayer(GradientLayer, 0);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (GradientLayer != null && GradientContentView != null)
            {
                // Turn off Animations
                CATransaction.Begin();
                CATransaction.DisableActions = true;

                if (e.PropertyName == GradientContentView.StartColorProperty.PropertyName)
                {
                    GradientLayer.Colors[0] = GradientContentView.StartColor.ToCGColor();
                }

                if (e.PropertyName == GradientContentView.EndColorProperty.PropertyName)
                {
                    GradientLayer.Colors[1] = GradientContentView.EndColor.ToCGColor();
                }

                if (e.PropertyName == VisualElement.WidthProperty.PropertyName ||
                    e.PropertyName == VisualElement.HeightProperty.PropertyName)
                {
                    GradientLayer.Frame = NativeView.Bounds;
                }

                if (e.PropertyName == GradientContentView.OrientationProperty.PropertyName)
                {
                    SetOrientation();
                }

                CATransaction.Commit();
            }
        }

        private void SetOrientation()
        {
            if (GradientContentView.Orientation == GradientOrientation.Horizontal)
            {
                GradientLayer.StartPoint = new CGPoint(0, 0.5);
                GradientLayer.EndPoint = new CGPoint(1, 0.5);
            }
            else
            {
                GradientLayer.StartPoint = new CGPoint(0.5, 0);
                GradientLayer.EndPoint = new CGPoint(0.5, 1);
            }
        }
    }
}
