using XenForms.Core.Messages;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Core.Toolbox.Actions
{
    public class OpenXamlAction : ToolboxAction
    {
        protected override void OnExecute(XenMessage message)
        {
            var res = message as OpenXamlResponse;
            if (res == null) return;

            Bus.Notify(new OpenXamlResponseReceived(res.XamlDefaults, res.FileName));
        }
    }
}