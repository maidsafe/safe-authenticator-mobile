using SafeAuthenticator.Controls;
using SafeAuthenticator.Models;
using SafeAuthenticator.Services;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels
{
    internal class BaseViewModel : ObservableObject
    {
        protected AuthService Authenticator => DependencyService.Get<AuthService>();

        protected INativeBrowserService OpeNativeBrowserService => DependencyService.Get<INativeBrowserService>();

        protected INativeProgressDialogService NativeProgressDialog => DependencyService.Get<INativeProgressDialogService>();
    }
}
