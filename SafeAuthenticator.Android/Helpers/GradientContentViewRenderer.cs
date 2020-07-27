// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using SafeAuthenticator.Droid.Helpers;
using SafeAuthenticatorApp.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(GradientContentView), typeof(GradientContentViewRenderer))]

namespace SafeAuthenticator.Droid.Helpers
{
    public class GradientContentViewRenderer : ViewRenderer<GradientContentView, Android.Views.View>
    {
        public GradientContentViewRenderer(Context context)
            : base(context)
        {
        }

        public GradientDrawable GradientDrawable { get; set; }

        private GradientContentView GradientContentView => Element;

        protected override void OnElementChanged(ElementChangedEventArgs<GradientContentView> e)
        {
            base.OnElementChanged(e);

            if (GradientContentView != null)
            {
                GradientDrawable = new GradientDrawable();
                GradientDrawable.SetColors(new int[] { GradientContentView.StartColor.ToAndroid(), GradientContentView.EndColor.ToAndroid() });
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (GradientDrawable != null && GradientContentView != null)
            {
                if (e.PropertyName == GradientContentView.StartColorProperty.PropertyName ||
                    e.PropertyName == GradientContentView.EndColorProperty.PropertyName)
                {
                    GradientDrawable.SetColors(new int[] { GradientContentView.StartColor.ToAndroid(), GradientContentView.EndColor.ToAndroid() });
                    Invalidate();
                }

                if (e.PropertyName == VisualElement.WidthProperty.PropertyName ||
                    e.PropertyName == VisualElement.HeightProperty.PropertyName ||
                    e.PropertyName == GradientContentView.OrientationProperty.PropertyName)
                {
                    Invalidate();
                }
            }
        }

        protected override bool DrawChild(Canvas canvas, global::Android.Views.View child, long drawingTime)
        {
            GradientDrawable.Bounds = canvas.ClipBounds;
            GradientDrawable.SetOrientation(GradientContentView.Orientation == GradientOrientation.Vertical ? GradientDrawable.Orientation.TopBottom
                : GradientDrawable.Orientation.LeftRight);
            GradientDrawable.Draw(canvas);
            return base.DrawChild(canvas, child, drawingTime);
        }
    }
}
