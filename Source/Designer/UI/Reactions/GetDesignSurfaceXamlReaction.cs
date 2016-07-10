using System;
using XenForms.Core.Messages;
using XenForms.Core.Networking;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    public class GetDesignSurfaceXamlReaction : XamarinFormsReaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            var xamlDefaults = string.Empty;

            try
            {
                xamlDefaults = XamlWriter.Save(App.CurrentDesignSurface);
            }
            catch (Exception)
            {
                // ignored
            }

            ctx.SetResponse<CreateWidgetResponse>(r =>
            {
                r.XamlDefaults = new[] {xamlDefaults};
            });
        }
    }
}
