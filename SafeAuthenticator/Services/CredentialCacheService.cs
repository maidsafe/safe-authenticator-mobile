using System;
using System.Linq;
using Xamarin.Auth;

namespace SafeAuthenticator.Services {
  internal class CredentialCacheService {
    private const string LocationKey = "Location";
    private const string PasswordKey = "Password";

    public void Delete() {
      try {
        var acctInfo = GetAccountInfo();
        AccountStore.Create().Delete(acctInfo, App.AppName);
      } catch (NullReferenceException) {
        // ignore acct not existing
      }
    }

    private static Account GetAccountInfo() {
      var acctInfo = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
      if (acctInfo == null) {
        throw new NullReferenceException("acctInfo");
      }

      return acctInfo;
    }

    public (string, string) Retrieve() {
      var acctInfo = GetAccountInfo();
      return (acctInfo.Properties[LocationKey], acctInfo.Properties[PasswordKey]);
    }

    public void Store(string location, string password) {
      var acctInfo = new Account {Username = "CachedAcct"};
      acctInfo.Properties.Add(LocationKey, location);
      acctInfo.Properties.Add(PasswordKey, password);
      AccountStore.Create().Save(acctInfo, App.AppName);
    }
  }
}
