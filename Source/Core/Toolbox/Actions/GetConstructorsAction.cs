using XenForms.Core.Messages;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Core.Toolbox.Actions
{
    public class GetConstructorsAction : ToolboxAction
    {
        protected override void OnExecute(XenMessage message)
        {
            var response = message as GetConstructorsResponse;
            if (response == null) return;

            var args = new ConstructorsReceived(response.Type);
            Bus.Notify(args);
        }
    }
}
