// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System.Threading.Tasks;
using NUnit.Framework;
using SafeAuthenticator.Native;

namespace SafeAuth.Tests
{
    [TestFixture]
    class LoginTest
    {
        [Test]
        public async Task RegesteredUserlogin()
        {
            string secret = Utils.GetRandomString(10);
            string password = Utils.GetRandomString(10);
            var (auth, session) = await Utils.CreateTestApp(secret, password);
            Assert.That(
                async () => await Utils.LoginTestApp(secret, password),
                Is.TypeOf<Authenticator>());
            Assert.DoesNotThrowAsync(async () => await Utils.LoginTestApp(secret, password));
        }

        [Test]
        public void UnRegesteredUserlogin()
        {
            string secret = Utils.GetRandomString(10);
            string password = Utils.GetRandomString(10);
            Assert.That(
                async () => await Utils.LoginTestApp(secret, password),
                Throws.TypeOf<FfiException>());
        }

        [Test]
        public async Task InvalidSecretlogin()
        {
            string secret = Utils.GetRandomString(10);
            string password = Utils.GetRandomString(10);
            var (auth, session) = await Utils.CreateTestApp(secret, password);
            Assert.That(
                async () => await Utils.LoginTestApp(Utils.GetRandomString(10), password),
                Throws.TypeOf<FfiException>());
        }

        [Test]
        public async Task InvalidPasswordlogin()
        {
            string secret = Utils.GetRandomString(10);
            string password = Utils.GetRandomString(10);
            var (auth, session) = await Utils.CreateTestApp(secret, password);
            Assert.That(
                async () => await Utils.LoginTestApp(secret, Utils.GetRandomString(10)),
                Throws.TypeOf<FfiException>());
        }
    }
}
