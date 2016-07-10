using System;
using System.Collections.Generic;
using XenForms.Core.Messages;
using XenForms.Core.Reflection;
using XenForms.Core.Toolbox.Actions;

namespace XenForms.Core.Toolbox.Workflow
{
    /// <summary>
    /// Used to create an association between an incoming <see cref="XenMessage"/> and a <see cref="ToolboxAction"/>.
    /// When a message is received from the designer, pass the raw message to the <see cref="Queue"/> method.
    /// The raw message will be inspected and the associated <see cref="XenMessage"/> type will be created. This will then
    /// be sent to any <see cref="ToolboxAction"/>s.
    /// </summary>
    public abstract class ToolboxWorkflow
    {
        protected readonly IXenMessageFinder MessageFinder;
        protected readonly ToolboxLogging Log;
        protected List<Registration> Registrar { get; set; }


        /// <summary>
        /// Inject dependencies into newly created objects.
        /// </summary>
        public static Action<object> GetServices;


        /// <summary>
        /// The number of associations that have been registered.
        /// </summary>
        public int Registrations => Registrar.Count;


        protected ToolboxWorkflow(IXenMessageFinder messageFinder, ToolboxLogging log)
        {
            MessageFinder = messageFinder;
            Log = log;
            Registrar = new List<Registration>();
        }


        /// <summary>
        /// Clear all registration
        /// </summary>
        public void Reset()
        {
            lock (Registrar)
            {
                Registrar.Clear();
            }
        }


        /// <summary>
        /// Associate a <see cref="XenMessage"/> to a <see cref="ToolboxAction"/>.
        /// </summary>
        /// <typeparam name="T">XenMessage</typeparam>
        /// <typeparam name="T1">Toolbox Action</typeparam>
        public virtual ToolboxWorkflow Register<T, T1>() where T : XenMessage, new() where T1 : ToolboxAction, new()
        {
            var registration = new Registration
            {
                Message = typeof(T),
                Action = typeof(T1)
            };

            lock (Registrar)
            {
                Registrar.Add(registration);
            }

            Log.Info($"{typeof(T).Name} will be handled by {typeof(T1).Name}.");

            return this;
        }


        /// <summary>
        /// Send the <paramref name="message"/> to the appropriate <see cref="ToolboxAction"/> for execution.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="next"></param>
        /// <returns>
        /// True if the message was executed; otherwise, false.
        /// </returns>
        public abstract bool Queue(string message, Action<XenMessage> next = null);


        /// <summary>
        /// Send the <paramref name="message"/> to the appropriate <see cref="ToolboxAction"/> for execution.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="next">Callback executed if this message has a <see cref="XenMessage.NextSuggestedMessage"/></param>
        /// <returns>
        /// True if the message was executed; otherwise, false.
        /// </returns>
        public bool Queue(XenMessage message, Action<XenMessage> next = null)
        {
            return Queue(message?.ToJson(), next);
        }


        /// <summary>
        /// Container to hold a <see cref="XenMessage"/> and <see cref="ToolboxAction"/> association.
        /// </summary>
        protected struct Registration
        {
            public Type Message { get; set; }
            public Type Action { get; set; }
        }
    }
}