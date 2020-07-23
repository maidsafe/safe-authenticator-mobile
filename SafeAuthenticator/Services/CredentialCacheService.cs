// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SafeAuthenticatorApp.Services
{
    internal class CredentialCacheService
    {
        private const string LocationKey = "Location";
        private const string PasswordKey = "Password";

        public void Delete()
        {
            try
            {
                SecureStorage.RemoveAll();
            }
            catch (NullReferenceException)
            {
                // ignore acct not existing
            }
        }

        public async Task<(string, string)> Retrieve()
        {
            var location = await SecureStorage.GetAsync(LocationKey);
            var password = await SecureStorage.GetAsync(PasswordKey);
            if (location == null && password == null)
            {
                throw new NullReferenceException("acctInfo");
            }

            return (location, password);
        }

        public async Task Store(string location, string password)
        {
            await SecureStorage.SetAsync(LocationKey, location);
            await SecureStorage.SetAsync(PasswordKey, password);
        }
    }
}
