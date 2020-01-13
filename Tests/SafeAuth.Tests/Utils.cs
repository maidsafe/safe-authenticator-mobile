// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeApp;
using SafeAuthenticator.Native;

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
            return await CreateTestApp(secret, password);
        }

        public static async Task<(Authenticator, Session)> CreateTestApp(string secret, string password)
        {
            var authReq = new SafeApp.Core.AuthReq
            {
                App = new SafeApp.Core.AppExchangeInfo
                { Id = GetRandomString(10), Name = GetRandomString(5), Scope = null, Vendor = GetRandomString(5) },
                AppContainer = true,
                Containers = new List<SafeApp.Core.ContainerPermissions>()
            };
            return await CreateTestApp(secret, password, authReq);
        }

        internal static async Task<(Authenticator, Session)> CreateTestApp(
            string secret,
            string password,
            SafeApp.Core.AuthReq authReq)
        {
            var auth = await Authenticator.CreateAccountAsync(secret, password);
            var (_, reqMsg) = await Session.EncodeAuthReqAsync(authReq);
            var ipcReq = await auth.DecodeIpcMessageAsync(reqMsg);
            Assert.That(ipcReq, Is.TypeOf<AuthIpcReq>());

            var authIpcReq = ipcReq as AuthIpcReq;
            var resMsg = await auth.EncodeAuthRespAsync(authIpcReq, true);
            var ipcResponse = await Session.DecodeIpcMessageAsync(resMsg);
            Assert.That(ipcResponse, Is.TypeOf<SafeApp.Core.AuthIpcMsg>());

            var authResponse = ipcResponse as SafeApp.Core.AuthIpcMsg;
            Assert.That(authResponse, Is.Not.Null);

            var session = await Session.AppConnectAsync(authReq.App.Id, resMsg);
            return (auth, session);
        }

        public static async Task<Authenticator> LoginTestApp(string secret, string password)
        {
            return await Authenticator.LoginAsync(secret, password);
        }

        public static async Task<string> RevokeAppAsync(Authenticator auth, string appId)
        {
            return await auth.AuthRevokeAppAsync(appId);
        }

        public static async Task<List<RegisteredApp>> GetRegisteredAppsAsync(Authenticator auth)
        {
            return await auth.AuthRegisteredAppsAsync();
        }

        public static SafeApp.Core.AuthReq CreateAuthRequest()
        {
            var authReq = new SafeApp.Core.AuthReq
            {
                App = new SafeApp.Core.AppExchangeInfo
                { Id = GetRandomString(10), Name = GetRandomString(5), Scope = null, Vendor = GetRandomString(5) },
                AppContainer = true,
                Containers = new List<SafeApp.Core.ContainerPermissions>()
            };
            return authReq;
        }

        public static SafeApp.Core.ContainersReq SetContainerPermission(
            SafeApp.Core.AuthReq authReq,
            string containerType)
        {
            var containerRequest = new SafeApp.Core.ContainersReq
            {
                App = authReq.App,
                Containers = new List<SafeApp.Core.ContainerPermissions>
                {
                    new SafeApp.Core.ContainerPermissions
                    {
                        ContName = containerType,
                        Access = new SafeApp.Core.PermissionSet
                            { Read = true, Insert = true, Delete = true, ManagePermissions = true, Update = true }
                    }
                }
            };
            return containerRequest;
        }

        public static byte[] GetRandomData(int length)
        {
            var data = new byte[length];
            Random.NextBytes(data);
            return data;
        }

        public static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
