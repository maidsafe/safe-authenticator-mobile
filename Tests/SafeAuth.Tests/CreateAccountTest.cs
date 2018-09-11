using NUnit.Framework;
using SafeAuthenticator.Native;
using System.Threading.Tasks;

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
            string invitation = Utils.GetRandomString(5);
            Assert.That(async () => await Utils.CreateTestApp(secret, password, invitation), Throws.TypeOf<FfiException>());
        }

        [Test]
        public void InvalidPassword()
        {
            string secret = Utils.GetRandomString(10);
            string password = null;
            string invitation = Utils.GetRandomString(5);
            Assert.That(async () => await Utils.CreateTestApp(secret, password, invitation), Throws.TypeOf<FfiException>());
        }

        [Test]
        public void InvalidInvitation()
        {
            string secret = Utils.GetRandomString(10);
            string password = Utils.GetRandomString(10);
            string invitation = null;
            Assert.That(async () => await Utils.CreateTestApp(secret, password, invitation), Throws.TypeOf<FfiException>());
        }

        [Test]
        public async Task SecretExists()
        {
            string secret = Utils.GetRandomString(10);
            string password = Utils.GetRandomString(10);
            string invitation = Utils.GetRandomString(5);
            var (auth, session) = await Utils.CreateTestApp(secret, password, invitation);
            auth.Dispose();

            password = Utils.GetRandomString(10);
            invitation = Utils.GetRandomString(5);
            Assert.That(async () => await Utils.CreateTestApp(secret, password, invitation), Throws.TypeOf<FfiException>());
        }
    }
}
