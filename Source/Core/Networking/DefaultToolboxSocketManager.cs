using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XenForms.Core.Diagnostics;
using XenForms.Core.Messages;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Toolbox.Shell;

namespace XenForms.Core.Networking
{
    public class DefaultToolboxSocketManager : ToolboxSocketManager
    {
        private readonly ToolboxSocket _socket;
        private readonly INetworkAdapter _adapter;
        private readonly IMessageBus _bus;
        private readonly Logging _log;
        private Action<string> _onTrace;
        private Action _onConnect;


        public override bool IsConnected => _socket?.IsConnected ?? false;
        public override ToolboxSocket Socket => _socket;


        public DefaultToolboxSocketManager(ToolboxSocket socket, INetworkAdapter adapter, IMessageBus bus, Logging log)
        {
            _socket = socket;
            _adapter = adapter;
            _bus = bus;
            _log = log;
        }


        public override bool Connect(string host, short port, Action onConnect = null, Action<string> onTrace = null)
        {
            if (IsConnected)
            {
                _log.Warn($"{nameof(Connect)} called while already connected. Call {nameof(Disconnect)} first.");
                return false;
            }

            if (!_adapter.IsResponsive(host))
            {
                onTrace?.Invoke($"The host {host} was unresponsive. Please check the address.");
                return false;
            }

            _onTrace = onTrace;
            _onConnect = onConnect;

            _socket.Trace += OnTrace;
            _socket.Connected += OnSocketConnected;
            _socket.Disconnected += OnSocketDisconnected;

            var address = ServiceEndpoint.FormatAddress(host, port);

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new InvalidOperationException($"The given address was invalid: {host}:{port}.");
            }

            _socket.Connect(address);
            return true;
        }


        public override void Disconnect()
        {
            if (!IsConnected)
            {
                _log.Warn($"{nameof(Disconnect)} called while not connected.");
                return;
            }

            _socket.Close();
            _log.Info("User chose to disconnect from designer.");
        }


        public override void ClearTraces()
        {
            _socket.Trace -= OnTrace;
            _onTrace = null;
        }


        private void OnSocketConnected(object sender, EventArgs e)
        {
            _onConnect?.Invoke();

            _log.Info($"Connected to {_socket.Endpoint}.");
            _bus.Notify(new InitializeWorkspace());
            _log.Trace($"{nameof(InitializeWorkspace)} published.");

            _onTrace = null;
        }


        private void OnTrace(object sender, SocketTraceEventArgs e)
        {
            _onTrace?.Invoke(e.Description);
            var desc = e.Description ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(e.Payload))
            {
                try
                {
                    var json = JObject.Parse(e.Payload);
                    desc += "\n" + json.ToString(Formatting.Indented);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            _log.Trace(desc);
        }


        private void OnSocketDisconnected(object sender, EventArgs e)
        {
            _bus.Notify(new DisconnectedFromDesigner());
            _log.Trace($"{nameof(DisconnectedFromDesigner)} published.");

            _socket.Connected -= OnSocketConnected;
            _socket.Disconnected -= OnSocketDisconnected;
            _socket.Trace -= OnTrace;

            _bus.Notify(new CleanUpWorkspace());
            _log.Trace($"{nameof(CleanUpWorkspace)} published.");
        }
    }
}
