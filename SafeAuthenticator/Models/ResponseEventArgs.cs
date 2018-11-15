using System;

namespace SafeAuthenticator.Models
{
    public class ResponseEventArgs : EventArgs
    {
        public bool Response { get; }

        public ResponseEventArgs(bool res)
        {
            Response = res;
        }
    }
}
