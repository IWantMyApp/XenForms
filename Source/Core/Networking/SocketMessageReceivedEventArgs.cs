using System;

namespace XenForms.Core.Networking
{
    public class SocketMessageReceivedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}
