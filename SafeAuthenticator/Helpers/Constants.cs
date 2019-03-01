namespace SafeAuthenticator.Helpers
{
    internal static class Constants
    {
        // StringStrength
        internal const int AccStrengthVeryWeak = 4;
        internal const int AccStrengthWeak = 8;
        internal const int AccStrengthSomeWhatSecure = 10;

        internal static readonly string AppName = "SAFE Authenticator";
        internal static readonly string IsTutorialComplete = "IsTutorialComplete";
        internal static readonly string AppOwnContainer = "App's own Container";

        // Authentication PopupState
        internal static readonly string None = "None";
        internal static readonly string Error = "Error";
        internal static readonly string Loading = "Loading";

        // Dialogs
        internal static readonly string AutoReconnectInfoDialog = "Enable this feature to automatically reconnect to the network." +
            " Your credentials will be securely stored on your device. Logging out will clear the credentials from memory.";

        // URL
        internal static readonly string ClaimTokenUrl = @"https://invite.maidsafe.net/";
        internal static readonly string ForumLinkUrl = @"https://safenetforum.org/t/trust-level-1-basic-user-requirements/15200";
        internal static readonly string PrivacyInfoUrl = @"https://safenetwork.tech/privacy/";
        internal static readonly string FaqUrl = @"https://safenetforum.org/t/safe-authenticator-faq/26683";
    }
}
