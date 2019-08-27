using System.Threading.Tasks;
using Foundation;
using SafeAuthenticator.iOS.Helpers;
using SafeAuthenticator.Services;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(AppHandler))]

namespace SafeAuthenticator.iOS.Helpers
{
    class AppHandler : IAppHandler
    {
        public Task<bool> LaunchApp(string uri)
        {
            return Task.FromResult(UIApplication.SharedApplication.OpenUrl(new NSUrl(uri)));
        }
    }
}
