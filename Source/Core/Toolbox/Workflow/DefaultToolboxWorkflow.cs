using System;
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
    public sealed class DefaultToolboxWorkflow : ToolboxWorkflow
    {
        private Action<XenMessage> _callback;


        public DefaultToolboxWorkflow(IXenMessageFinder messageFinder, ToolboxLogging log) 
            : base(messageFinder, log)
        {
            // ignored
        }


        /// <summary>
        /// Send the <paramref name="message"/> to the appropriate <see cref="ToolboxAction"/> for execution.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="next"></param>
        /// <returns>
        /// True if the message was executed; otherwise, false.
        /// </returns>
        public override bool Queue(string message, Action<XenMessage> next = null)
        {
            _callback = next;

            var parsed = false;
            var executed = false;
            XenMessage xMessage = null;

            if (string.IsNullOrWhiteSpace(message))
            {
                parsed = false;
            }
            else
            {
                parsed = XenMessage.TryParseFromAction(MessageFinder, message, out xMessage);
            }

            if (parsed)
            {
                var matches = Registrar.FindAll(r => r.Message.Name.Equals(xMessage.Action));

                if (matches.Count == 0)
                {
                    ExecuteNextMessage(xMessage);
                }

                foreach (var match in matches)
                {
                    var action = Activator.CreateInstance(match.Action) as ToolboxAction;
                    if (action == null) continue;

                    GetServices?.Invoke(action);
                    action.Execute(xMessage);
                    executed = true;

                    ExecuteNextMessage(xMessage);
                }
            }

            if (!executed)
            {
                Log.Warn("A received message was not dispatched.");
            }

            return executed;
        }


        private void ExecuteNextMessage(XenMessage response)
        {
            if (string.IsNullOrWhiteSpace(response?.NextSuggestedMessage)) return;

            var nextType = MessageFinder.Find(response.NextSuggestedMessage);
            if (nextType == null) return;

            var nextMessage = XenMessage.Create(nextType);

            if (nextMessage != null)
            {
                // copy over the widget id.
                var wResponse = response as IWidgetMessage;
                var wNextRequest = nextMessage as IWidgetMessage;

                if (wResponse != null && wNextRequest != null)
                {
                    wNextRequest.WidgetId = wResponse.WidgetId;
                }

                _callback?.Invoke(nextMessage);
            }
        }
    }
}