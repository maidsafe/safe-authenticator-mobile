using System.Threading.Tasks;
using NUnit.Framework;
using SafeAuthenticator.Native;

namespace SafeAuth.Tests
{
    [TestFixture]
    class CreateAccountTest
    {
        [Test]
        public async Task CreateAccountValid()
        {
            var (auth, session) = await Utils.CreateTestApp();
            Assert.That(() => auth, Is.TypeOf<Authenticator>());
        }

        [Test]
        public void InvalidSecret()
        {
            string secret = null;
            string password = Utils.GetRandomString(10);
            Assert.That(
                async () => await Utils.CreateTestApp(secret, password),
                Throws.TypeOf<FfiException>());
        }

        [Test]
        public void InvalidPassword()
        {
            string secret = Utils.GetRandomString(10);
            string password = null;
            Assert.That(
                async () => await Utils.CreateTestApp(secret, password),
                Throws.TypeOf<FfiException>());
        }

        [Test]
        public async Task SecretExists()
        {
            string secret = Utils.GetRandomString(10);
            string password = Utils.GetRandomString(10);
            var (auth, session) = await Utils.CreateTestApp(secret, password);
            auth.Dispose();

            password = Utils.GetRandomString(10);
            Assert.That(
                async () => await Utils.CreateTestApp(secret, password),
                Throws.TypeOf<FfiException>());
        }
    }
}
