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
using System.Threading.Tasks;

namespace SafeAuthenticator.Native
{
    public partial interface IAuthBindings
    {
        bool AuthIsMock();

        Task AuthReconnectAsync(IntPtr auth);

        Task AuthSetConfigDirPathAsync(string newPath);

        void AuthFree(IntPtr auth);

        Task AuthRmRevokedAppAsync(IntPtr auth, string appId);

        Task<List<AppExchangeInfo>> AuthRevokedAppsAsync(IntPtr auth);

        Task<List<RegisteredApp>> AuthRegisteredAppsAsync(IntPtr auth);

        Task<List<AppAccess>> AuthAppsAccessingMutableDataAsync(IntPtr auth, byte[] mdName, ulong mdTypeTag);

        Task<string> EncodeShareMDataRespAsync(IntPtr auth, ref ShareMDataReq req, uint reqId, bool isGranted);

        Task<string> AuthRevokeAppAsync(IntPtr auth, string appId);

        Task AuthFlushAppRevocationQueueAsync(IntPtr auth);

        Task<string> EncodeUnregisteredRespAsync(uint reqId, bool isGranted);

        Task<string> EncodeAuthRespAsync(IntPtr auth, ref AuthReq req, uint reqId, bool isGranted);

        Task<string> EncodeContainersRespAsync(IntPtr auth, ref ContainersReq req, uint reqId, bool isGranted);

        Task AuthInitLoggingAsync(string outputFileNameOverride);

        Task<string> AuthConfigDirPathAsync();
    }
}
