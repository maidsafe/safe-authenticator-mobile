using Foundation;
using SafariServices;
using SafeAuthenticator.Controls;
using SafeAuthenticator.iOS.Helpers;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppleNativeBrowserService))]

namespace SafeAuthenticator.iOS.Helpers
{
    public class AppleNativeBrowserService : INativeBrowserService
    {
        public void LaunchNativeEmbeddedBrowser(string url)
        {
            var destination = new NSUrl(url);
            var sfViewController = new SFSafariViewController(destination);

            var window = UIApplication.SharedApplication.KeyWindow;
            var controller = window.RootViewController;
            controller.PresentViewController(sfViewController, true, null);
        }
    }
}
