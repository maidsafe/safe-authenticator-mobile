using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils;
using JetBrains.Annotations;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Native;

namespace SafeAuthenticator.Models {
  public class RegisteredAppModel : ObservableObject, IComparable, IEquatable<RegisteredAppModel> {
    [PublicAPI]
    public AppExchangeInfo AppInfo { get; }

    public string AppName => AppInfo.Name;
    public string AppVendor => AppInfo.Vendor;
    public string AppId => AppInfo.Id;
    [PublicAPI]
    public ObservableRangeCollection<ContainerPermissionsModel> Containers { get; }

    public RegisteredAppModel(AppExchangeInfo appInfo, IEnumerable<ContainerPermissions> containers) {
      AppInfo = appInfo;
      Containers = containers.Select(
        x => new ContainerPermissionsModel {
          Access = new PermissionSetModel {
            Read = x.Access.Read,
            Insert = x.Access.Insert,
            Update = x.Access.Update,
            Delete = x.Access.Delete,
            ManagePermissions = x.Access.ManagePermissions
          },
          ContainerName = x.ContName
        }).ToObservableRangeCollection();
    }

    public int CompareTo(object obj) {
      if (!(obj is RegisteredAppModel other)) {
        throw new NotSupportedException();
      }

      return string.CompareOrdinal(AppInfo.Name, other.AppInfo.Name);
    }

    public bool Equals(RegisteredAppModel other) {
      if (ReferenceEquals(null, other)) {
        return false;
      }

      return ReferenceEquals(this, other) || AppInfo.Id.Equals(other.AppInfo.Id);
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) {
        return false;
      }

      if (ReferenceEquals(this, obj)) {
        return true;
      }

      return obj.GetType() == GetType() && ((RegisteredAppModel)obj).AppInfo.Id == AppInfo.Id;
    }

    public override int GetHashCode() {
      return 0;
    }
  }
}
