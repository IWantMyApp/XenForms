using System.Linq;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Reflection;
using XenForms.Core.Widgets;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    public class GetWidgetEventsReaction : XamarinFormsReaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            var request = ctx.Get<GetWidgetEventsRequest>();
            if (request == null) return;

            var pair = Surface[request.WidgetId];

            var events = pair?
                .VisualElement?
                .GetType()
                .GetPublicEvents()
                .Select(XenEvent.Create)
                .ToArray();

            if (events == null) return;

            ctx.SetResponse<GetWidgetEventsResponse>(r =>
            {
                r.Events = events;
                r.WidgetId = request.WidgetId;
            });
        }
    }
}