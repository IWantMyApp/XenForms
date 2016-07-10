using XenForms.Core.Messages;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Core.Toolbox.Actions
{
    public class GetWidgetEventsAction : ToolboxAction
    {
        protected override void OnExecute(XenMessage message)
        {
            var response = message as GetWidgetEventsResponse;

            if (response == null)
            {
                Log.Warn($"{nameof(GetWidgetEventsResponse)} was not correctly parsed.");
                return;
            }

            var root = VisualTree.Root;

            if (root == null)
            {
                Log.Warn("Cannot attach events when a visual tree does not exist.");
                return;
            }

            var match = root.Find(response.WidgetId);
            match.Events = response.Events;

            Bus.Notify(new ShowWidgetEvents(match));
            Log.Trace($"Properties set for {match.Events}.");
        }
    }
}
