using System;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Reflection;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    public class CallConstructorReaction : SetPropertyReaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            if (ctx.Request == null) return;

            var req = ctx.Get<CallConstructorRequest>();
            if (req == null) return;

            var error = string.Empty;
            var success = false;
            var obj = XenConstructorMethods.Construct(TypeFinder, req.Constructor);

            if (obj != null)
            {
                try
                {
                    SetPropertyValue(req.WidgetId, req.Property.Path, obj, false, false);
                }
                catch (Exception ex)
                {
                    error = ex.ToString();
                    success = false;
                }

                success = true;
            }

            ctx.SetResponse<CallConstructorResponse>(res =>
            {
                res.ErrorMessage = error;
                res.Successful = success;
            });
        }
    }
}
