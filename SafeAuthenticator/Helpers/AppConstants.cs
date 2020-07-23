// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using JetBrains.Annotations;

namespace SafeAuthenticatorApp.Helpers
{
    [PublicAPI]
    public static class AppConstants
    {
        public const ulong AsymNonceLen = 24;
        public const ulong AsymPublicKeyLen = 32;
        public const ulong SymEncKeyLen = 32;
        public const ulong BlsPublicKeyLen = 48;
        public const ulong AsymSecretKeyLen = 32;
        public const ulong DirTag = 15000;
        public const ulong MaidsafeTag = 5483000;
        public const string MDataMetaDataKey = "_metadata";
        public const ulong NullObjectHandle = 0;
        public const ulong SignPublicKeyLen = 32;
        public const ulong SignSecretKeyLen = 64;
        public const ulong SymKeyLen = 32;
        public const ulong SymNonceLen = 24;
        public const ulong XorNameLen = 32;
    }
}
