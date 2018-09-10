using Android.Content;
using SafeAuthenticator.Controls;
using SafeAuthenticator.Droid.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PaddedEntry), typeof(PaddedEntryRenderer))]

namespace SafeAuthenticator.Droid.Helpers {
  internal class PaddedEntryRenderer : EntryRenderer {

    public PaddedEntryRenderer(Context context) : base(context) {
      AutoPackage = false;
    }

    protected override void OnElementChanged(ElementChangedEventArgs<Entry> e) {
      base.OnElementChanged(e);

      // ReSharper disable once UseNullPropagation
      if (Control != null) {
      }
    }
  }
}
