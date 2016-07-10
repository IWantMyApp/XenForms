using System;
using XenForms.Core.Diagnostics;
using XenForms.Core.Messages;

namespace XenForms.Core.Networking
{
    /// <summary>
    /// Base API for connecting to a <see cref="DesignServer"/>.
    /// This class is used exclusively by a toolbox.
    /// </summary>
    public abstract class ToolboxSocket
    {
        /// <summary>
        /// The <see cref="DesignServer"/> address.
        /// This is set after a successful connect.
        /// </summary>
        public string Endpoint { get; protected set; }


        /// <summary>
        /// Returns true if connected to a <see cref="DesignServer"/>; otherwise, false.
        /// </summary>
        public virtual bool IsConnected { get; protected set; }


        /// <summary>
        /// Emits logging and debugging messages that can be subscribed to.
        /// </summary>
        public virtual event EventHandler<SocketTraceEventArgs> Trace;
        public virtual event EventHandler<EventArgs> Connected;
        public virtual event EventHandler<EventArgs> Disconnected;
        public virtual event EventHandler<SocketMessageReceivedEventArgs> MessageReceived; 


        /// <summary>
        /// Connect to a <see cref="DesignServer"/> listening at <paramref name="address"/>.
        /// </summary>
        /// <param name="address"></param>
        public virtual void Connect(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new InvalidOperationException("A design server address must be specified.");
            }

            Endpoint = address;
            ConnectImplementation();
        }


        /// <summary>
        /// Connect to a <see cref="DesignServer" />.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="path"></param>
        public virtual void Connect(string ip, short port, string path = "/")
        {
            var formatted = ServiceEndpoint.FormatAddress(ip, port, path);

            if (formatted == null)
            {
                throw new InvalidOperationException("The design server address was invalid.");
            }

            Endpoint = formatted;
            ConnectImplementation();
        }


        /// <summary>
        /// Send a <paramref name="message"/> to the design server.
        /// The XenForms design protocol is a series of JSON messages.
        /// </summary>
        /// <param name="message"></param>
        public virtual void Send(string message)
        {
            SendImplementation(message);
        }


        /// <summary>
        /// Send a <see cref="XenMessage"/> to the design server.
        /// The message is serialized to JSON before sending.
        /// </summary>
        /// <param name="message"></param>
        public virtual void Send(XenMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.MessageId))
            {
                message.MessageId = Guid.NewGuid().ToString();
            }

            SendImplementation(message.ToJson());
        }


        /// <summary>
        /// Close the current connection to a <see cref="DesignServer"/>.
        /// </summary>
        public virtual void Close()
        {
            CloseImplementation(SocketCloseCode.Normal, "The toolbox disconnected.");
        }


        /// <summary>
        /// Close the current connection to a <see cref="DesignServer"/>
        /// </summary>
        /// <param name="code"></param>
        /// <param name="reason"></param>
        public virtual void Close(SocketCloseCode code, string reason)
        {
            CloseImplementation(code, reason);
        }


        protected abstract void SendImplementation(string payload);
        protected abstract void ConnectImplementation();
        protected abstract void CloseImplementation(SocketCloseCode code, string reason);


        protected virtual void OnTrace(XenLogLevel level, string description, string payload)
        {
            var args = new SocketTraceEventArgs(description, payload, level);
            OnTrace(args);
        }


        protected virtual void OnTrace(XenLogLevel level, string description)
        {
            var args = new SocketTraceEventArgs(description, null, level);
            OnTrace(args);
        }


        protected virtual void OnTrace(SocketTraceEventArgs e)
        {
            Trace?.Invoke(this, e);
        }


        protected virtual void OnConnectedToDesignServer()
        {
            IsConnected = true;
            OnTrace(XenLogLevel.Info, "Connected to design server.");
            Connected?.Invoke(this, EventArgs.Empty);
        }


        protected virtual void OnDisconnectedFromDesignServer()
        {
            IsConnected = false;
            OnTrace(XenLogLevel.Info, "Disconnected from design server.");
            Disconnected?.Invoke(this, EventArgs.Empty);
        }


        protected virtual void OnMessageReceived(SocketMessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }
    }
}