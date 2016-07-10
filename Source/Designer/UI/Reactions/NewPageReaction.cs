using XenForms.Core.Messages;
using XenForms.Core.Networking;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    public class NewPageReaction : XamarinFormsReaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            var req = ctx.Get<NewPageRequest>();
            if (req == null) return;

            App.ConnectionPage.PushNewDesignSurface();

            ctx.SetResponse<OkResponse>(res =>
            {
                res.Suggest<GetVisualTreeRequest>();
            });
        }
    }
}