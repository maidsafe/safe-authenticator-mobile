using Android.App;
using Android.Content;
using Android.Support.CustomTabs;
using SafeAuthenticator.Controls;
using SafeAuthenticator.Droid.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidNativeBrowserService))]

namespace SafeAuthenticator.Droid.Helpers
{
    public class AndroidNativeBrowserService : INativeBrowserService
    {
        public void LaunchNativeEmbeddedBrowser(string url)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var activity = Forms.Context as Activity;
#pragma warning restore CS0618 // Type or member is obsolete

            if (activity == null)
            {
                return;
            }

            var mgr = new CustomTabsActivityManager(activity);

            mgr.CustomTabsServiceConnected += (name, client) =>
            {
                mgr.LaunchUrl(url);
            };

            if (!mgr.BindService())
            {
                var uri = Android.Net.Uri.Parse(url);
                var intent = new Intent(Intent.ActionView, uri);
                activity.StartActivity(intent);
            }
        }
    }
}
