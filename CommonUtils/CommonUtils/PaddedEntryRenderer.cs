#if __MOBILE__
#if __IOS__
using CommonUtils;
using CoreGraphics;
using SafeAuthenticator.iOS.Helpers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

#elif __ANDROID__
using Android.Content;
using CommonUtils;
using SafeAuthenticator.iOS.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

#endif

[assembly: ExportRenderer(typeof(PaddedEntry), typeof(PaddedEntryRenderer))]

namespace SafeAuthenticator.iOS.Helpers {
  internal class PaddedEntryRenderer : EntryRenderer {
#if __ANDROID__
    public PaddedEntryRenderer(Context context) : base(context) {
      AutoPackage = false;
    }
#endif

    protected override void OnElementChanged(ElementChangedEventArgs<Entry> e) {
      base.OnElementChanged(e);

      // ReSharper disable once UseNullPropagation
      if (Control != null) {
#if __IOS__
        Control.LeftView = new UIView(new CGRect(0, 0, 40, 0));
        Control.LeftViewMode = UITextFieldViewMode.Always;
#elif __ANDROID__
// Control.SetPadding(40, 0, 0, 0);
#endif
      }
    }
  }
}

#endif
