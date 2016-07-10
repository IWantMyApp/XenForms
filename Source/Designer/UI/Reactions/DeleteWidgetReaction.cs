using XenForms.Core.Messages;
using XenForms.Core.Networking;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    public class DeleteWidgetReaction : XamarinFormsReaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            var request = ctx.Get<DeleteWidgetRequest>();
            if (request == null) return;

            var pair = Surface[request.WidgetId];
            if (pair == null) return;

            if (!pair.XenWidget.CanDelete)
            {
                return;
            }

            DesignThread.Invoke(() =>
            {
                Surface.Remove(request.WidgetId);
            });

            ctx.SetResponse<OkResponse>(r =>
            {
                r.Suggest<GetVisualTreeRequest>();
            });
        }
    }
}
