using System;

namespace SafeAuthenticator.Controls
{
    public interface INativeProgressDialogService
    {
        IDisposable ShowNativeDialog(string message, string title = null);
    }
}
