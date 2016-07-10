using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using XenForms.Core.Diagnostics;

namespace XenForms.Core.Networking
{
    internal sealed class DesignServerBehavior : WebSocketBehavior
    {
        private readonly DesignServer _server;


        public DesignServerBehavior(DesignServer server)
        {
            _server = server;
            EmitOnPing = true;
        }


        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);

            #pragma warning disable 618

            // e.Is* does not seem to work as of 12/30/15

            if (e.Type == Opcode.Binary)
            {
                _server.OnTrace(XenLogLevel.Warn, @"Binary messages are unsupported. Ignored.");
                return;
            }


            if (e.Type == Opcode.Ping || e.Type == Opcode.Pong)
            {
                _server.OnTrace(XenLogLevel.Trace, @"Ping/Pong received from toolbox.");
                return;
            }


            if (e.Type == Opcode.Text)
            {
                // The data property contains the text message received from the toolbox.
                // It's not known if the data is valid JSON.

                var msgEvent = new SocketMessageReceivedEventArgs
                {
                    Message = e.Data
                };

                _server.OnMessage(msgEvent);
            }

            #pragma warning restore 618

        }


        protected override void OnOpen()
        {
            base.OnOpen();

            var e = new SocketConnectedEventArgs
            {
                Address = Context.Host,
                UniqueId = ID
            };

            _server.OnToolboxConnected(e);
            _server.OnTrace(XenLogLevel.Info, $"Toolbox {Context.Host} connected.");

            if (_server.ConnectionQueue.Count > 1)
            {
                _server.OnTrace(XenLogLevel.Warn, "Disconnecting Toolbox. Multiple toolboxes cannot be connected to the designer.");
                Context.WebSocket.Close();
            }
        }


        protected override void OnClose(CloseEventArgs wssArgs)
        {
            base.OnClose(wssArgs);

            var level = XenLogLevel.Info;
            var remoteHost = Context.Host;

            if (!wssArgs.WasClean)
            {
                level = XenLogLevel.Error;
            }

            var e = new SocketClosedEventArgs
            {
                Address = Context.Host,
                UniqueId = ID
            };

            _server.OnToolboxDisconnected(e);
            _server.OnTrace(level, $"Toolbox {remoteHost} disconnected.{Environment.NewLine}Code: {wssArgs.Code}. Reason: {wssArgs.Reason}");
        }


        protected override void OnError(ErrorEventArgs wssArgs)
        {
            base.OnError(wssArgs);

            var e = new SocketErrorEventArgs
            {
                Message = wssArgs.Message
            };

            _server.OnError(e);
        }
    }
}