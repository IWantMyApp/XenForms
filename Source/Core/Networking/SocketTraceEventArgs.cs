using System;
using XenForms.Core.Diagnostics;

namespace XenForms.Core.Networking
{
    public class SocketTraceEventArgs : EventArgs
    {
        public string Payload { get; set; }
        public string Description { get; set; }
        public XenLogLevel Level { get; set; }


        public SocketTraceEventArgs(string description, string payload, XenLogLevel level = XenLogLevel.Info)
        {
            Description = description;
            Payload = payload;
            Level = level;
        }
    }
}
