// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using UIKit;
using Xamarin.Forms;

namespace SafeAuthenticator.iOS.Extensions
{
    public static class KeyboardExtensions
    {
        public static UIKeyboardType ToNative(this Keyboard input)
        {
            if (input == Keyboard.Chat)
            {
                return UIKeyboardType.Twitter;
            }
            if (input == Keyboard.Text)
            {
                return UIKeyboardType.ASCIICapable;
            }
            if (input == Keyboard.Numeric)
            {
                return UIKeyboardType.NumberPad;
            }
            if (input == Keyboard.Telephone)
            {
                return UIKeyboardType.PhonePad;
            }
            if (input == Keyboard.Url)
            {
                return UIKeyboardType.Url;
            }
            if (input == Keyboard.Email)
            {
                return UIKeyboardType.EmailAddress;
            }
            return UIKeyboardType.Default;
        }
    }
}
