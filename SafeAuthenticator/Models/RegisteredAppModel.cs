// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Native;

namespace SafeAuthenticator.Models
{
    public class RegisteredAppModel : ObservableObject, IComparable, IEquatable<RegisteredAppModel>
    {
        [PublicAPI]
        public AppExchangeInfo AppInfo { get; }

        public string AppName => AppInfo.Name;

        public string AppVendor => AppInfo.Vendor;

        public string AppId => AppInfo.Id;

        public string CircleColor { get; set; }

        public AppPermissions AppPermissions { get; }

        [PublicAPI]
        public ObservableRangeCollection<ContainerPermissionsModel> Containers { get; }

        public RegisteredAppModel(
            AppExchangeInfo appInfo,
            IEnumerable<ContainerPermissions> containers,
            AppPermissions? appPermissions)
        {
            AppInfo = appInfo;
            if (appPermissions.HasValue)
                AppPermissions = appPermissions.Value;
            Containers = containers.Select(
                x => new ContainerPermissionsModel
                {
                    Access = new PermissionSetModel
                    {
                        Read = x.Access.Read,
                        Insert = x.Access.Insert,
                        Update = x.Access.Update,
                        Delete = x.Access.Delete,
                        ManagePermissions = x.Access.ManagePermissions
                    },
                    ContainerName = Utilities.FormatContainerName(x.ContName, appInfo.Id)
                }).ToObservableRangeCollection();

            Containers = Containers.OrderBy(c => c.ContainerName).ToObservableRangeCollection();
            CircleColor = Utilities.GetRandomColor(AppName.Length);
        }

        public int CompareTo(object obj)
        {
            if (!(obj is RegisteredAppModel other))
            {
                throw new NotSupportedException();
            }

            return string.CompareOrdinal(AppInfo.Name, other.AppInfo.Name);
        }

        public bool Equals(RegisteredAppModel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return ReferenceEquals(this, other) || AppInfo.Id.Equals(other.AppInfo.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && ((RegisteredAppModel)obj).AppInfo.Id == AppInfo.Id;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
