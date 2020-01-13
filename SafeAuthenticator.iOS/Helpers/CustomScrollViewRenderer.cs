// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System.ComponentModel;
using CoreGraphics;
using SafeAuthenticator.Controls;
using SafeAuthenticator.iOS.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomScrollView), typeof(CustomScrollViewRenderer))]

namespace SafeAuthenticator.iOS.Helpers
{
    class CustomScrollViewRenderer : ScrollViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            ScrollEnabled = ((CustomScrollView)e.NewElement).IsScrollEnabled;
            ((CustomScrollView)e.NewElement).PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != "IsScrollEnabled")
                return;

            var isScrollEnabled = ((CustomScrollView)sender).IsScrollEnabled;

            // IsScrollEnabled is a custom property used to enable/disable scroll
            // Reset the ScrollView to its original position if false
            if (!isScrollEnabled)
                ContentOffset = new CGPoint(0, 0);
            ScrollEnabled = isScrollEnabled;
        }
    }
}
