using System;
using BigTed;
using SafeAuthenticator.Controls;
using SafeAuthenticator.iOS.Helpers;
using SafeAuthenticator.Models;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppleNativeProgressDialogService))]
namespace SafeAuthenticator.iOS.Helpers
{
    class AppleNativeProgressDialogService : INativeProgressDialogService
    {
        public void HideNativeDialog()
        {
            BTProgressHUD.Dismiss();
        }

        public IDisposable ShowNativeDialog(string message, string title)
        {
            BTProgressHUD.Show(message, -1, ProgressHUD.MaskType.Black);
            return new DisposableAction(() => { BTProgressHUD.Dismiss(); });
        }
    }
}
