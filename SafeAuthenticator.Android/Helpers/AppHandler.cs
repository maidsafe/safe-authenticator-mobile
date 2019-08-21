using System.Threading.Tasks;
using Android.Content;
using SafeAuthenticator.Droid.Helpers;
using SafeAuthenticator.Services;
using Activity = Plugin.CurrentActivity.CrossCurrentActivity;

[assembly: Xamarin.Forms.Dependency(typeof(AppHandler))]

namespace SafeAuthenticator.Droid.Helpers
{
    class AppHandler : IAppHandler
    {
        public Task<bool> LaunchApp(string uri)
        {
            bool result = false;

            try
            {
                var parsedUri = Android.Net.Uri.Parse(uri);
                var intent = new Intent(Intent.ActionView, parsedUri);
                intent.AddFlags(ActivityFlags.NewTask);
                Activity.Current.AppContext.StartActivity(intent);
                result = true;
            }
            catch (ActivityNotFoundException)
            {
                result = false;
            }

            return Task.FromResult(result);
        }
    }
}
