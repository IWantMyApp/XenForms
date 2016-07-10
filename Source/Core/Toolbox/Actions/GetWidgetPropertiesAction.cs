using XenForms.Core.Messages;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Core.Toolbox.Actions
{
    public class GetWidgetPropertiesAction : ToolboxAction
    {
        protected override void OnExecute(XenMessage message)
        {
            var res = message as GetWidgetPropertiesResponse;

            if (res == null)
            {
                Log.Warn($"{nameof(GetWidgetPropertiesResponse)} was not correctly parsed.");
                return;
            }

            var root = VisualTree.Root;

            if (root == null)
            {
                Log.Warn("Cannot attach properties to a widget when the visual tree does not exist.");
                return;
            }

            var match = root.Find(res.Widget.Id);
            match.Properties = res.Properties;

            Log.Trace($"Properties set for {match.Name}.");

            Bus.Notify(new ShowWidgetPropertyEditors(match));
            Bus.Notify(new ReplacedWidgetCollection(match));

            Log.Trace($"{nameof(ShowWidgetPropertyEditors)} published.");
            Log.Trace($"{nameof(ReplacedWidgetCollection)} published.");
        }
    }
}
