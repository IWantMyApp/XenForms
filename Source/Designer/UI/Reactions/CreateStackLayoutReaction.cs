using System;
using Xamarin.Forms;
using XenForms.Core.Messages;
using XenForms.Core.Networking;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    public class CreateStackLayoutReaction : XamarinFormsReaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            var request = ctx.Get<CreateStackLayoutRequest>();
            if (request == null) return;

            var orientation = StackOrientation.Vertical;

            if (string.IsNullOrWhiteSpace(request.Orientation) || request.Orientation.ToLower() == "vertical")
            {
                orientation = StackOrientation.Vertical;
            }
            else if (request.Orientation.ToLower() == "horizontal")
            {
                orientation = StackOrientation.Horizontal;
            }

            var view = new StackLayout
            {
                Orientation = orientation,
                Spacing = request.Spacing
            };

            var target = Surface[request.ParentId];
            if (target == null) return;

            var attached = false;

            DesignThread.Invoke(() =>
            {
                attached = Surface.SetParent(view, target);
            });

            if (!attached) return;

            var pair = Surface[view.Id];

            var xamlDefaults = string.Empty;

            try
            {
                xamlDefaults = XamlWriter.Save(new StackLayout());
            }
            catch (Exception)
            {
                // ignored
            }

            ctx.SetResponse<CreateWidgetResponse>(r =>
            {
                r.XamlDefaults = new[] { xamlDefaults };
                r.Widget = pair.XenWidget;
                r.Parent = target.XenWidget;
                r.Suggest<GetVisualTreeRequest>();
            });
        }
    }
}
