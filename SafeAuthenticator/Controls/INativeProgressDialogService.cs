using System;
using System.Collections.Generic;
using System.Text;

namespace SafeAuthenticator.Controls
{
    public interface INativeProgressDialogService
    {
        IDisposable ShowNativeDialog(string message, string title = null);

        void HideNativeDialog();
    }
}
