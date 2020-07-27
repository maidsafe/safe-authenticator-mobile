﻿// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System.Threading.Tasks;
using Android.Content;
using SafeAuthenticator.Droid.Helpers;
using SafeAuthenticatorApp.Services;
using Activity = Plugin.CurrentActivity.CrossCurrentActivity;

[assembly: Xamarin.Forms.Dependency(typeof(AppHandler))]

namespace SafeAuthenticator.Droid.Helpers
{
    class AppHandler : IAppHandler
    {
        public Task<bool> LaunchApp(string uri)
        {
            bool result;
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
