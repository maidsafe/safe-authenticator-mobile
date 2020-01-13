// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Globalization;
using System.Text;
using SafeAuthenticator.Models;
using Xamarin.Forms;

namespace SafeAuthenticator.Controls.Converters
{
    class FormatContainerPermissionsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var access = value as PermissionSetModel;
            var permissionsFieldText = new StringBuilder();

            if (access.Read)
                permissionsFieldText.Append("Read.");
            if (access.Insert)
                permissionsFieldText.Append("Insert.");
            if (access.Update)
                permissionsFieldText.Append("Update.");
            if (access.Delete)
                permissionsFieldText.Append("Delete.");
            if (access.ManagePermissions)
                permissionsFieldText.Append("Manage Permissions.");

            return permissionsFieldText.ToString().TrimEnd('.').Replace(".", ", ");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
