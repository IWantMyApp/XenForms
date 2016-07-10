using System;
using System.Reflection;
using XenForms.Core.Designer;
using XenForms.Core.Messages;
using XenForms.Core.Networking;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    public class AddSupportedTypesReaction : XamarinFormsReaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            var req = ctx.Get<AddSupportedTypeRequest>();
            if (req == null) return;

            var res = XenMessage.Create<AddSupportedTypeResponse>();

            try
            {
                if (TypeRegistrar.Instance.IsRegistered(req.Type))
                {
                    res.AlreadyRegistered = true;
                    res.DisplayMessage = $"The type '{req.Type.FullName} is already registered.";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(req.Type?.FullName))
                    {
                        var match = TypeFinder.Find(req.Type.FullName);

                        if (match == null)
                        {
                            res.DisplayMessage = $"The type '{req.Type.FullName}' was not found.";
                        }
                        else
                        {
                            if (match.GetTypeInfo().IsValueType)
                            {
                                res.Added = TypeRegistrar.Instance.AddType(req.Type);
                                res.Suggest<GetVisualTreeRequest>();

                                if (res.Added)
                                {
                                    res.DisplayMessage = $"The type '{req.Type.FullName}' can now be used.";
                                }
                            }
                            else
                            {
                                res.DisplayMessage = "You can only add value types in the beta.";
                            }
                        }
                    }
                    else
                    {
                        res.DisplayMessage = "You must enter a type name.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.UnhandledExceptionOccurred = true;
                res.ExceptionMessage = ex.ToString();
            }

            ctx.Response = res;
        }
    }
}