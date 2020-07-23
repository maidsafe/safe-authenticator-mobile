// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using Xamarin.Forms;

namespace SafeAuthenticatorApp.Controls.Behaviour
{
    public class ListViewNoSelectionBehavior : Behavior<ListView>
    {
        protected override void OnAttachedTo(ListView listview)
        {
            listview.ItemSelected += OnItemSelected;
            base.OnAttachedTo(listview);
        }

        protected override void OnDetachingFrom(ListView listview)
        {
            listview.ItemSelected -= OnItemSelected;
            base.OnDetachingFrom(listview);
        }

        private static void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }
}
