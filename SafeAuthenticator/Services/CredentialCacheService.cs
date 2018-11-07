using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SafeAuthenticator.Services
{
    internal class CredentialCacheService {
    private const string LocationKey = "Location";
    private const string PasswordKey = "Password";

    public void Delete() {
      try {
        SecureStorage.RemoveAll();
      }
      catch (NullReferenceException) {
        // ignore acct not existing
      }
    }

    public async Task<(string,string)> Retrieve() {
      var location = await SecureStorage.GetAsync(LocationKey);
      var password = await SecureStorage.GetAsync(PasswordKey);
      if (location==null && password==null) {
        throw new NullReferenceException("acctInfo");
      }
      return (location, password);
    }

    public async Task Store(string location, string password) {
      await SecureStorage.SetAsync(LocationKey, location);
      await SecureStorage.SetAsync(PasswordKey, password);
    }
  }
}
