using JetBrains.Annotations;

namespace SafeAuthenticator.Helpers
{
    [PublicAPI]
    public static class AppConstants
    {
        public const ulong AsymNonceLen = 24;
        public const ulong AsymPublicKeyLen = 32;
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
        public const int AccStrengthVeryWeak = 4;
        public const int AccStrengthWeak = 8;
        public const int AccStrengthSomeWhatSecure = 10;
    }
}
