using XenForms.Core.Messages;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Core.Toolbox.Actions
{
    public class AttachedPropertiesReceivedAction : ToolboxAction
    {
        protected override void OnExecute(XenMessage message)
        {
            var res = message as GetAttachedPropertiesResponse;

            if (res == null)
            {
                Log.Warn($"{nameof(GetAttachedPropertiesResponse)} was null and not correctly parsed.");
                return;
            }

            var root = VisualTree.Root;

            if (root == null)
            {
                Log.Warn("Cannot assigned attached properties when a visual tree does not exist.");
                return;
            }

            var match = root.Find(res.Widget.Id);
            match.AttachedProperties = res.Widget.AttachedProperties;

            Bus.Notify(new ShowAttachedProperties(res.Widget));
        }
    }
}