// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using Android.Support.Design.Widget;
using SafeAuthenticator.Controls;
using SafeAuthenticator.Droid.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(EntryMoveNextEffect), nameof(EntryMoveNextEffect))]

namespace SafeAuthenticator.Droid.Helpers
{
    public class EntryMoveNextEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            // Check if the attached element is of the expected type and has the NextEntry
            // property set. if so, configure the keyboard to indicate there is another entry
            // in the form and the dismiss action to focus on the next entry
            if (Element is MaterialEntry xfControl)
            {
                var entry = ((TextInputLayout)Control).EditText;

                entry.ImeOptions = (xfControl.ReturnType == ReturnType.Next) ?
                    Android.Views.InputMethods.ImeAction.Next :
                    Android.Views.InputMethods.ImeAction.Done;

                if (xfControl.NextEntry != null)
                {
                    entry.EditorAction += (sender, args) =>
                    {
                        xfControl.OnNext();
                    };
                }
            }
        }

        protected override void OnDetached()
        {
            // Intentionally empty
        }
    }
}
