// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using SafeAuthenticator.Controls;
using SafeAuthenticator.iOS.Helpers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(EntryMoveNextEffect), nameof(EntryMoveNextEffect))]

namespace SafeAuthenticator.iOS.Helpers
{
    class EntryMoveNextEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var entry = (UITextField)Control;

            // Check if the attached element is of the expected type and has the NextEntry
            // property set. if so, configure the keyboard to indicate there is another entry
            // in the form and the dismiss action to focus on the next entry
            if (Element is MaterialEntry xfControl && xfControl.ReturnType != ReturnType.Default)
            {
                entry.ReturnKeyType = (xfControl.ReturnType == ReturnType.Next) ?
                    UIReturnKeyType.Next :
                    UIReturnKeyType.Done;
            }
        }

        protected override void OnDetached()
        {
            // Intentionally empty
        }
    }
}
