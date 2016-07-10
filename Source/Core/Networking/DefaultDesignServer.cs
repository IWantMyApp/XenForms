using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using XenForms.Core.Diagnostics;
using XenForms.Core.Messages;

namespace XenForms.Core.Networking
{
    public sealed class DefaultDesignServer : DesignServer
    {
        private readonly object _lock = new object();
        private readonly INetworkAdapter _networkAdapter;
        private WebSocketServer _wss;

        private readonly ManualResetEventSlim _cancellation = new ManualResetEventSlim();

        public override bool IsListening => _wss?.IsListening ?? false;
        public override string ListeningOn => ServiceEndpoint.FormatAddress(_networkAdapter.GetIp(), ServiceEndpoint.Port, ServiceEndpoint.DesignerPath);
        public override string ConnectedToolbox => ConnectionQueue.Values.FirstOrDefault();
        public override bool IsToolboxConnected => !string.IsNullOrWhiteSpace(ConnectedToolbox);


        public DefaultDesignServer(INetworkAdapter networkAdapter)
        {
            _networkAdapter = networkAdapter;
        }


        public override void Start()
        {
            lock (_lock)
            {
                _wss = new WebSocketServer(ServiceEndpoint.Port);
                _wss.AddWebSocketService(ServiceEndpoint.DesignerPath, () => new DesignServerBehavior(this));
                _wss.WaitTime = ServiceEndpoint.PingWaitTime;
                _cancellation.Reset();
            }

            Task.Run(() =>
            {
                if (_wss != null)
                {
                    lock (_lock)
                    {
                        _wss?.Start();
                    }

                    OnServerStarted();
                }

                _cancellation.Wait();
                OnTrace(XenLogLevel.Info, "Cancellation event received; stopping server.");
            });
        }


        public override void Stop()
        {
            try
            {
                lock (_lock)
                {
                    if (!_cancellation.IsSet)
                    {
                        _cancellation.Set();
                        var reason = new DesignServerStoppedOk().ToJson();
                        _wss.Stop(CloseStatusCode.Normal, reason);
                        _wss = null;
                    }
                }
            }
            catch (Exception e)
            {
                OnTrace(XenLogLevel.Error, $"An error occurred while stopping the web server:\n{e}");
            }
        }


        public override void Send(string message)
        {
            try
            {
                if (!IsListening) return;
                _wss.WebSocketServices.Broadcast(message);
            }
            catch (Exception e)
            {
                OnTrace(XenLogLevel.Error, $"An error occurred while sending:\n{message}\n{e}");
            }
        }


        #region Events


        internal override void OnToolboxDisconnected(SocketClosedEventArgs e)
        {
            if (ConnectionQueue.ContainsKey(e.UniqueId))
            {
                ConnectionQueue.Remove(e.UniqueId);
            }

            base.OnToolboxDisconnected(e);
        }


        internal override void OnToolboxConnected(SocketConnectedEventArgs e)
        {
            if (!ConnectionQueue.ContainsKey(e.UniqueId))
            {
                ConnectionQueue.Add(e.UniqueId, e.Address);
            }

            base.OnToolboxConnected(e);
        }


        #endregion
    }
}
