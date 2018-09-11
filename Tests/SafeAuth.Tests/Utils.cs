using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeAuthenticator.Native;
using SafeApp;

namespace SafeAuth.Tests
{
    class Utils
    {
        private static readonly Random Random = new Random();

        public static async Task<string> AuthenticateContainerRequest(Authenticator auth, string ipcMsg, bool allow)
        {
            var ipcReq = await auth.DecodeIpcMessageAsync(ipcMsg);
            Assert.That(ipcReq, Is.TypeOf<ContainersIpcReq>());

            var response = await auth.EncodeContainersRespAsync(ipcReq as ContainersIpcReq, allow);
            return response;
        }

        public static async Task<(Authenticator, Session)> CreateTestApp()
        {
            string secret = GetRandomString(10);
            string password = GetRandomString(10);
            string invitation = GetRandomString(5);
            return await CreateTestApp(secret, password, invitation);
        }

        public static async Task<(Authenticator, Session)> CreateTestApp(string secret, string password, string invitation)
        {
            var authReq = new SafeApp.Utilities.AuthReq
            {
                App = new SafeApp.Utilities.AppExchangeInfo { Id = GetRandomString(10), Name = GetRandomString(5), Scope = null, Vendor = GetRandomString(5) },
                AppContainer = true,
                Containers = new List<SafeApp.Utilities.ContainerPermissions>()
            };
            return await CreateTestApp(secret, password, invitation, authReq);
        }

        internal static async Task<(Authenticator, Session)> CreateTestApp(string secret, string password, string invitation, SafeApp.Utilities.AuthReq authReq)
        {
            var auth = await Authenticator.CreateAccountAsync(secret, password, invitation);
            var (_, reqMsg) = await Session.EncodeAuthReqAsync(authReq);
            var ipcReq = await auth.DecodeIpcMessageAsync(reqMsg);
            Assert.That(ipcReq, Is.TypeOf<AuthIpcReq>());

            var authIpcReq = ipcReq as AuthIpcReq;
            var resMsg = await auth.EncodeAuthRespAsync(authIpcReq, true);
            var ipcResponse = await Session.DecodeIpcMessageAsync(resMsg);
            Assert.That(ipcResponse, Is.TypeOf<SafeApp.Utilities.AuthIpcMsg>());

            var authResponse = ipcResponse as SafeApp.Utilities.AuthIpcMsg;
            Assert.That(authResponse, Is.Not.Null);

            var session = await Session.AppRegisteredAsync(authReq.App.Id, authResponse.AuthGranted);
            return (auth, session);
        }

        public static async Task<Authenticator> LoginTestApp(string secret, string password)
        {
            return await Authenticator.LoginAsync(secret, password);
        }

        public static SafeApp.Utilities.AuthReq CreateContainerAuthRequest()
        {
            var authReq = new SafeApp.Utilities.AuthReq
            {
                App = new SafeApp.Utilities.AppExchangeInfo { Id = GetRandomString(10), Name = GetRandomString(5), Scope = null, Vendor = GetRandomString(5) },
                AppContainer = true,
                Containers = new List<SafeApp.Utilities.ContainerPermissions>()
            };
            return authReq;
        }

        public static SafeApp.Utilities.ContainersReq SetContainerPermission(SafeApp.Utilities.AuthReq authReq, string containerType)
        {
            var containerRequest = new SafeApp.Utilities.ContainersReq
            {
                App = authReq.App,
                Containers = new List<SafeApp.Utilities.ContainerPermissions>
                {
                    new SafeApp.Utilities.ContainerPermissions  { ContName = containerType, Access = new SafeApp.Utilities.PermissionSet { Read = true } }
                }
            };
            return containerRequest;
        }

        public static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
