using System;
using WebSocketSharp;
using XenForms.Core.Diagnostics;

namespace XenForms.Core.Networking
{
    public sealed class DefaultToolboxSocket : ToolboxSocket, IDisposable
    {
        private WebSocket _socket;
        private bool _timedOut;


        protected override void ConnectImplementation()
        {
            try
            {
                _timedOut = false;

                _socket = new WebSocket(Endpoint)
                {
                    EmitOnPing = true,
                    WaitTime = ServiceEndpoint.PingWaitTime
                };

                _socket.OnOpen += OnOpen;
                _socket.OnClose += OnClose;
                _socket.OnError += OnError;
                _socket.OnMessage += OnMessage;

                OnTrace(XenLogLevel.Info, $"Connecting to {Endpoint}.");
                _socket.Connect();
            }
            catch (Exception ex)
            {
                OnTrace(XenLogLevel.Error, ex.Message);
                Close();
            }
        }


        public void Dispose()
        {
            if (_socket != null)
            {
                var disposable = _socket as IDisposable;
                disposable.Dispose();
                IsConnected = false;
            }
        }


        protected override void SendImplementation(string payload)
        {
            OnTrace(XenLogLevel.Trace, "Sending message", payload);
            _socket.Send(payload);
        }


        protected override void CloseImplementation(SocketCloseCode code, string reason)
        {
            if (_socket != null)
            {
                OnTrace(XenLogLevel.Info, $"Closing connection to {Endpoint}.");

                var status = CloseStatusCode.Undefined;

                switch (code)
                {
                    case SocketCloseCode.Normal:
                        status = CloseStatusCode.Normal;
                        break;
                    case SocketCloseCode.Undefined:
                        status = CloseStatusCode.Undefined;
                        break;
                    case SocketCloseCode.UnsupportedData:
                        status = CloseStatusCode.UnsupportedData;
                        break;
                }

                _socket.Close(status, reason);
            }
        }


        private void OnMessage(object sender, MessageEventArgs e)
        {
            #pragma warning disable 618

            // e.Is* doesn't seem to work as of 12/30/15

            if (e.Type == Opcode.Close)
            {
                OnDisconnectedFromDesignServer();
            }


            if (e.Type == Opcode.Ping || e.Type == Opcode.Pong)
            {
                OnTrace(XenLogLevel.Trace, "Ping/Pong received from designer.");
                return;
            }


            if (e.Type == Opcode.Binary)
            {
                Close(SocketCloseCode.UnsupportedData, "The XenForms protocol must be used.");
            }

            #pragma warning restore 618

            OnMessageReceived(new SocketMessageReceivedEventArgs { Message = e.Data });
            OnTrace(XenLogLevel.Trace, "Message Received", e.Data);
        }


        private void OnClose(object sender, CloseEventArgs e)
        {
            if (_timedOut) return;

            OnDisconnectedFromDesignServer();

            var clean = (e.WasClean ? "Clean" : "Unclean") + " close";
            OnTrace(XenLogLevel.Info, $"Code: {e.Code}. Reason: {e.Reason}.{Environment.NewLine}{clean}.");
        }


        private void OnError(object sender, ErrorEventArgs e)
        {
            if (e?.Exception == null) return;

            _timedOut = e.Exception.Message.Contains("period of time");
            OnTrace(XenLogLevel.Error, e.Exception.Message);
        }


        private void OnOpen(object sender, EventArgs e)
        {
            OnConnectedToDesignServer();
        }
    }
}