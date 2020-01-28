// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using Android.App;
using SafeAuthenticator.Controls;
using SafeAuthenticator.Droid.Helpers;
using SafeAuthenticator.Models;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidNativeProgressDialogService))]

namespace SafeAuthenticator.Droid.Helpers
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class AndroidNativeProgressDialogService : INativeProgressDialogService
    {
        public IDisposable ShowNativeDialog(string message, string title)
        {
            ProgressDialog progress = new ProgressDialog((Activity)Forms.Context)
            {
                Indeterminate = true,
            };
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
#pragma warning restore CS0618 // Type or member is obsolete
}
