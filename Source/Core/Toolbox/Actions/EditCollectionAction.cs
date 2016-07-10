using XenForms.Core.Messages;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Core.Toolbox.Actions
{
    public class EditCollectionAction : ToolboxAction
    {
        protected override void OnExecute(XenMessage message)
        {
            var response = message as EditCollectionResponse;
            if (response == null) return;

            var args = new EditCollectionResponseReceived(response.Type, response.Successful, response.Message);
            Bus.Notify(args);
        }
    }
}
