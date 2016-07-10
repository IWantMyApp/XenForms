using XenForms.Core.Messages;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Core.Toolbox.Actions
{
    public class AddSupportedTypeAction : ToolboxAction
    {
        protected override void OnExecute(XenMessage message)
        {
            var res = message as AddSupportedTypeResponse;
            if (res == null) return;

            var args = new AddSupportedTypeEvent
            {
                Added = res.Added,
                AlreadyRegistered = res.AlreadyRegistered,
                DisplayMessage = res.DisplayMessage
            };

            Bus.Notify(args);
        }
    }
}