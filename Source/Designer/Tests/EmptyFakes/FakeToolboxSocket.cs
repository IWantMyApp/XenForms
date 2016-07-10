using XenForms.Core.Networking;

namespace XenForms.Designer.Tests.EmptyFakes
{
    sealed class FakeToolboxSocket : ToolboxSocket
    {
        public string Incoming { get; set; }


        public void InvokeConnectEvent()
        {
            OnConnectedToDesignServer();
        }


        public void InvokeDisconnectEvent()
        {
            OnDisconnectedFromDesignServer();
        }


        protected override void SendImplementation(string message)
        {
            Incoming = message;
        }

        protected override void ConnectImplementation()
        {
        }

        protected override void CloseImplementation(SocketCloseCode code, string reason)
        {
        }
    }
}