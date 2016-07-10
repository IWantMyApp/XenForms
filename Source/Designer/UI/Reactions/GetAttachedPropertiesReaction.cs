using System.Collections.Generic;
using System.Linq;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Widgets;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    public class GetAttachedPropertiesReaction : XamarinFormsReaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            if (ctx.Request == null) return;

            var req = ctx.Get<GetAttachedPropertiesRequest>();
            if (req == null) return;

            var target = Surface[req.WidgetId];

            var all = new List<XenProperty>();
            var parents = ElementHelper.GetParents(target.XenWidget);

            foreach (var parent in parents)
            {
                var view = Surface[parent.Id].VisualElement;

                var aps = ElementHelper.GetAttachedProperties(view).ToArray();
                if (aps == null || aps.Length == 0) continue;

                foreach (var ap in aps)
                {
                    // skip dupe attached props
                    var processed = all.Any(a => a.XamlPropertyName == ap.XamlPropertyName);
                    if (processed) continue;

                    var xt = Descriptor.CreateType(ap.GetMethod);
                    xt.Descriptor |= XenPropertyDescriptors.AttachedProperty;
                    var apval = ap.GetMethod.Invoke(null, new object[] {target.VisualElement});

                    var xp = new XenProperty
                    {
                        XamlPropertyName = ap.XamlPropertyName,
                        CanWrite = true,
                        CanRead = true,
                        XenType = xt,
                        Value = apval,
                        Path = new[] { ap.Field.Name }
                    };

                    all.Add(xp);
                }
            }

            target.XenWidget.AttachedProperties = all.ToArray();

            ctx.SetResponse<GetAttachedPropertiesResponse>(r =>
            {
                r.Widget = target.XenWidget;
            });
        }
    }
}
