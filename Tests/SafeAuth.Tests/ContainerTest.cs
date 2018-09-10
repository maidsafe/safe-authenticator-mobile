using System.Threading.Tasks;
using NUnit.Framework;
using SafeApp;

namespace SafeAuth.Tests
{
    [TestFixture]
    class ContainerTest
    {
        [Test]
        public async Task AllowPermission()
        {
            string secret = Utils.GetRandomString(10);
            string password = Utils.GetRandomString(10);

            var authReq = Utils.CreateContainerAuthRequest();
            var (auth, session) = await Utils.CreateTestApp(secret, password, Utils.GetRandomString(5), authReq);
            Assert.Throws<SafeApp.Utilities.FfiException>(() => session.AccessContainer.GetMDataInfoAsync("_public").GetAwaiter().GetResult());
    
            var containerRequest = Utils.SetContainerPermission(authReq, "_public");
            var (reqId, msg) = await Session.EncodeContainerRequestAsync(containerRequest);
            var responseMsg = await Utils.AuthenticateContainerRequest(auth, msg, true);
            var ipcMsg = await Session.DecodeIpcMessageAsync(responseMsg);
            Assert.AreEqual(typeof(SafeApp.Utilities.ContainersIpcMsg), ipcMsg.GetType());

            var containerResponse = ipcMsg as SafeApp.Utilities.ContainersIpcMsg;
            Assert.AreEqual(reqId, containerResponse?.ReqId);

            await session.AccessContainer.RefreshAccessInfoAsync();
            var mDataInfo = await session.AccessContainer.GetMDataInfoAsync("_public");
            Assert.That(mDataInfo.TypeTag, Is.EqualTo(15000));
        }

        [Test]
        public async Task DenyPermission()
        {
            string secret = Utils.GetRandomString(10);
            string password = Utils.GetRandomString(10);

            var authReq = Utils.CreateContainerAuthRequest();
            var (auth, session) = await Utils.CreateTestApp(secret, password, Utils.GetRandomString(5), authReq);
            Assert.Throws<SafeApp.Utilities.FfiException>(() => session.AccessContainer.GetMDataInfoAsync("_videos").GetAwaiter().GetResult());

            var containerRequest = Utils.SetContainerPermission(authReq, "_videos");
            var (_, msg) = await Session.EncodeContainerRequestAsync(containerRequest);
            var responseMsg = await Utils.AuthenticateContainerRequest(auth, msg, false);
            Assert.That(async () => await Session.DecodeIpcMessageAsync(responseMsg), Throws.TypeOf<SafeApp.Utilities.IpcMsgException>());
            Assert.Throws<SafeApp.Utilities.FfiException>(() => session.AccessContainer.GetMDataInfoAsync("_videos").GetAwaiter().GetResult());
        }

        [Test]
        public async Task UpdatePermission()
        {
            string secret = Utils.GetRandomString(10);
            string password = Utils.GetRandomString(10);

            var authReq = Utils.CreateContainerAuthRequest();
            var (auth, session) = await Utils.CreateTestApp(secret, password, Utils.GetRandomString(5), authReq);

            var containerRequest = Utils.SetContainerPermission(authReq, "_public");
            var (_, msg) = await Session.EncodeContainerRequestAsync(containerRequest);
            var responseMsg = await Utils.AuthenticateContainerRequest(auth, msg, true);
            Assert.That(async () => await session.AccessContainer.GetMDataInfoAsync("_public"), Is.TypeOf<SafeApp.Utilities.MDataInfo>());
            Assert.Throws<SafeApp.Utilities.FfiException>(() => session.AccessContainer.GetMDataInfoAsync("_videos").GetAwaiter().GetResult());

            containerRequest = Utils.SetContainerPermission(authReq, "_videos");
            (_, msg) = await Session.EncodeContainerRequestAsync(containerRequest);
            responseMsg = await Utils.AuthenticateContainerRequest(auth, msg, true);
            await session.AccessContainer.RefreshAccessInfoAsync();
            Assert.That(async () => await session.AccessContainer.GetMDataInfoAsync("_videos"), Is.TypeOf<SafeApp.Utilities.MDataInfo>());
        }
    }
}
