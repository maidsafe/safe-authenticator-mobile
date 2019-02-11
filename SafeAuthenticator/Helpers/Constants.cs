namespace SafeAuthenticator.Helpers
{
    static class Constants
    {
        // StringStrength
        public const int AccStrengthVeryWeak = 4;
        public const int AccStrengthWeak = 8;
        public const int AccStrengthSomeWhatSecure = 10;

        internal static readonly string AppName = "SAFE Authenticator";
        internal static readonly string IsFirstLaunch = "IsFirstLaunch";

        // Authentication PopupState
        internal static readonly string None = "None";
        internal static readonly string Error = "Error";
        internal static readonly string Loading = "Loading";
    }
}
