using System;
using Android.App;
using SafeAuthenticator.Controls;
using SafeAuthenticator.Droid.Helpers;
using SafeAuthenticator.Models;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidNativeProgressDialogService))]

namespace SafeAuthenticator.Droid.Helpers
{
    public class AndroidNativeProgressDialogService : INativeProgressDialogService
    {
        public IDisposable ShowNativeDialog(string message, string title)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            ProgressDialog progress = new ProgressDialog((Activity)Forms.Context);
#pragma warning restore CS0618 // Type or member is obsolete

            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetTitle(title);
            progress.SetMessage(message);
            progress.SetCancelable(false);
            progress.Show();
            return new DisposableAction(() =>
            {
                progress.Dismiss();
                progress.Dispose();
            });
        }
    }
}
