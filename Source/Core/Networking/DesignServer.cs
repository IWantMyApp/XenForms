using System;
using System.Collections.Generic;
using XenForms.Core.Diagnostics;

namespace XenForms.Core.Networking
{
    /// <summary>
    /// Design Server API.
    /// </summary>
    public abstract class DesignServer
    {
        internal readonly Dictionary<string, string> ConnectionQueue = new Dictionary<string, string>();


        /// <summary>
        /// Returns true if the server is accepting connections from a toolbox; otherwise, false.
        /// </summary>
        public virtual bool IsListening { get; protected set; }


        /// <summary>
        /// Return the IP address, port, and path the server is accepting connections on.
        /// </summary>
        public virtual string ListeningOn { get; protected set; }


        /// <summary>
        /// Return a list of connected toolboxes.
        /// </summary>
        public virtual string ConnectedToolbox { get; protected set; }


        /// <summary>
        /// Returns true if the server has connected toolboxes; otherwise, false.
        /// </summary>
        public virtual bool IsToolboxConnected { get; protected set; }


        /// <summary>
        /// Start listening and accepting toolbox connections.
        /// </summary>
        public abstract void Start();


        /// <summary>
        /// Stop listening and accepting toolbox connections.
        /// Any connected toolbox connections will be closed.
        /// </summary>
        public abstract void Stop();


        /// <summary>
        /// Send a message to the toolbox.
        /// </summary>
        /// <param name="message"></param>
        public abstract void Send(string message);

        public event EventHandler<EventArgs> ServerStarted; 
        public event EventHandler<SocketConnectedEventArgs> ToolboxConnected;
        public event EventHandler<SocketClosedEventArgs> ToolboxDisconnected;
        public event EventHandler<SocketTraceEventArgs> Trace;
        public event EventHandler<SocketErrorEventArgs> Error;
        public event EventHandler<SocketMessageReceivedEventArgs> Message;  

        internal virtual void OnError(SocketErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }


        internal virtual void OnTrace(XenLogLevel level, string description, string payload)
        {
            var e = new SocketTraceEventArgs(description, payload, level);
            Trace?.Invoke(this, e);
        }


        internal virtual void OnTrace(XenLogLevel level, string description)
        {
            var e = new SocketTraceEventArgs(description, null, level);
            Trace?.Invoke(this, e);
        }


        internal virtual void OnTrace(SocketTraceEventArgs e)
        {
            Trace?.Invoke(this, e);
        }


        internal virtual void OnToolboxDisconnected(SocketClosedEventArgs e)
        {
            ToolboxDisconnected?.Invoke(this, e);
        }


        internal virtual void OnToolboxConnected(SocketConnectedEventArgs e)
        {
            ToolboxConnected?.Invoke(this, e);
        }


        internal virtual void OnMessage(SocketMessageReceivedEventArgs e)
        {
            Message?.Invoke(this, e);
        }


        protected virtual void OnServerStarted()
        {
            ServerStarted?.Invoke(this, EventArgs.Empty);
        }
    }
}