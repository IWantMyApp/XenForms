using System;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.XAML;
using XenForms.Designer.XamarinForms.UI.Pages;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    public class SaveXamlReaction : XamarinFormsReaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            var req = ctx.Get<SaveXamlRequest>();
            if (req == null) return;

            var success = true;
            var xaml = string.Empty;
            var error = string.Empty;

            try
            {
                var writer = new XenXamlWriter();
                xaml = writer.Save(App.CurrentDesignSurface);
            }
            catch (Exception ex)
            {
                success = false;
                error = ex.ToString();
            }

            ctx.SetResponse<SaveXamlResponse>(res =>
            {
                res.Xaml = xaml;
                res.Successful = success;
                res.Error = error;
            });
        }
    }
}
