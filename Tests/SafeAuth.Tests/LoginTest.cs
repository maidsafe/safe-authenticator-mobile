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
            var (auth, session) = await Utils.CreateTestApp(secret, password, Utils.GetRandomString(5));
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
            var (auth, session) = await Utils.CreateTestApp(secret, password, Utils.GetRandomString(5));
            Assert.That(
                async () => await Utils.LoginTestApp(Utils.GetRandomString(10), password),
                Throws.TypeOf<FfiException>());
        }

        [Test]
        public async Task InvalidPasswordlogin()
        {
            string secret = Utils.GetRandomString(10);
            string password = Utils.GetRandomString(10);
            var (auth, session) = await Utils.CreateTestApp(secret, password, Utils.GetRandomString(5));
            Assert.That(
                async () => await Utils.LoginTestApp(secret, Utils.GetRandomString(10)),
                Throws.TypeOf<FfiException>());
        }
    }
}
