using XenForms.Core.Messages;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Core.Toolbox.Actions
{
    public class CallConstructorAction : ToolboxAction
    {
        protected override void OnExecute(XenMessage message)
        {
            var res = message as CallConstructorResponse;
            if (res == null) return;

            var args = new ConstructorCalled(res.Successful);
            Bus.Notify(args);
        }
    }
}
