using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Widgets;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    public class OpenXamlReaction : XamarinFormsReaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            var r = ctx.Get<OpenXamlRequest>();
            if (string.IsNullOrWhiteSpace(r?.Xaml)) return;

            var xaml = new List<string>();

            App.ConnectionPage?.PushNewPage(r.Xaml);
            var root = Surface.SetDesignSurface(App.CurrentDesignSurface);

            var tree = root.GetNodeAndDescendants().ToArray();

            foreach (var item in tree)
            {
                try
                {
                    var pair = Surface[item.Id];
                    if (pair == null) continue;

                    var vet = pair.VisualElement.GetType();
                    var view = Activator.CreateInstance(vet) as VisualElement;
                    var output = XamlWriter.Save(view);

                    if (string.IsNullOrWhiteSpace(output)) continue;
                    xaml.Add(output);
                }
                catch (Exception e)
                {
                    App.ConnectionPage?.ShowError(e);
                    break;
                }
            }

            ctx.SetResponse<OpenXamlResponse>(res =>
            {
                res.FileName = r.FileName;
                res.XamlDefaults = xaml.ToArray();
                res.Suggest<GetVisualTreeRequest>();
            });
        }
    }
}