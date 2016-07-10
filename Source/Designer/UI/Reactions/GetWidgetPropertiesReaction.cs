using Xamarin.Forms;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Widgets;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    public class GetWidgetPropertiesReaction<TVisualElement> : GetPropertiesReaction<TVisualElement>
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            if (ctx.Request == null) return;

            var req = ctx.Get<GetWidgetPropertiesRequest>();
            if (string.IsNullOrWhiteSpace(req?.WidgetId)) return;

            var props = GetXenProperties(req.WidgetId, req.IncludeValues) ?? new XenProperty[] { };

            foreach (var prop in props)
            {
                var isource = prop.Value as XenImageSource;
                if (isource != null)
                {
                    prop.Value = isource.FileName;
                }
            }

            var pair = Surface[req.WidgetId];

            ctx.SetResponse<GetWidgetPropertiesResponse>(res =>
            {
                res.Widget = pair.XenWidget;
                res.Properties = props;
            });
        }
    }
}