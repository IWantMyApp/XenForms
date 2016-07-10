using XenForms.Core.Messages;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Core.Toolbox.Actions
{
    public class DesignerReadyAction : ToolboxAction
    {
        protected override void OnExecute(XenMessage message)
        {
            var res = message as DesignerReady;
            if (res == null) return;

            Bus.Notify(new DesignerReadyEvent());
        }
    }
}
