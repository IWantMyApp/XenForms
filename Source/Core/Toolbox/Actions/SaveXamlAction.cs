using XenForms.Core.Messages;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Core.Toolbox.Actions
{
    public class SaveXamlAction : ToolboxAction
    {
        protected override void OnExecute(XenMessage message)
        {
            var res = message as SaveXamlResponse;
            if (res == null) return;

            var appEvent = new SaveXamlReceived
            {
                Error = res.Error,
                Successful = res.Successful,
                Xaml = res.Xaml
            };

            Bus.Notify(appEvent);
        }
    }
}