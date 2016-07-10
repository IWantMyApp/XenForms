using Ninject;
using XenForms.Core.Messages;
using XenForms.Core.Toolbox.Shell;

namespace XenForms.Core.Toolbox.Actions
{
    public abstract class ToolboxAction
    {
        [Inject]
        public ToolboxLogging Log { get; set; }

        [Inject]
        public IMessageBus Bus { get; set; }


        public void Execute(XenMessage message)
        {
            Log.Info($"Executing {message.Action} with {GetType().Name}.");
            OnExecute(message);
            Log.Info($"Finished executing {message.Action} with {GetType().Name}.");
        }


        protected abstract void OnExecute(XenMessage message);
    }
}