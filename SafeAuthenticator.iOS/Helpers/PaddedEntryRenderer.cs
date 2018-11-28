using CoreGraphics;
using SafeAuthenticator.Controls;
using SafeAuthenticator.iOS.Helpers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PaddedEntry), typeof(PaddedEntryRenderer))]
namespace SafeAuthenticator.iOS.Helpers
{
    internal class PaddedEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.LeftView = new UIView(new CGRect(0, 0, 40, 0));
                Control.LeftViewMode = UITextFieldViewMode.Always;
            }
        }
    }
}
