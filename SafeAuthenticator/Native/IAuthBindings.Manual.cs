// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SafeAuthenticator.Native
{
    public partial interface IAuthBindings
    {
        void CreateAccount(string locator, string secret, Action disconnnectedCb, Action<FfiResult, IntPtr, GCHandle> cb);

        void Login(string locator, string secret, Action disconnnectedCb, Action<FfiResult, IntPtr, GCHandle> cb);

        Task<IpcReq> DecodeIpcMessage(IntPtr authPtr, string uri);

        Task<IpcReq> UnRegisteredDecodeIpcMsgAsync(string msg);
    }
}
