using System;

namespace XenForms.Core.Networking
{
    public abstract class ToolboxSocketManager
    {
        public abstract bool IsConnected { get; }
        public abstract bool Connect(string host, short port, Action onConnect = null, Action<string> onTrace = null);
        public abstract void Disconnect();
        public abstract void ClearTraces();
        public abstract ToolboxSocket Socket { get; }
    }
}