using XenForms.Core.Messages;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Core.Toolbox.Actions
{
    public class XamlResponseAction : ToolboxAction
    {
        protected override void OnExecute(XenMessage message)
        {
            var res = message as XamlResponse;
            if (res == null) return;

            Bus.Notify(new XamlElementDefaultsReceived(res.XamlDefaults));
        }
    }
}