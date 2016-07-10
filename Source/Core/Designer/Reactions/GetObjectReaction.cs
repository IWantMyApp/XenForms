using System;
using System.Linq;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Reflection;
using XenForms.Core.Widgets;

namespace XenForms.Core.Designer.Reactions
{
    public class GetObjectReaction<TVisualElement> : GetPropertiesReaction<TVisualElement>
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            if (ctx.Request == null) return;

            var req = ctx.Get<GetObjectRequest>();
            if (req == null) return;

            var props = GetXenProperties(req.WidgetId, true, req.Path);

            if (!props.Any())
            {
                return;
            }

            var prop = props[0];
            if (prop.XenType.Descriptor.HasFlag(XenPropertyDescriptors.Enumerable))
            {
                if (prop.XenType.Descriptor.HasFlag(XenPropertyDescriptors.Enumerable))
                {
                    // grab index value; if null, return without creating an ObjectResponse.
                    var index = ReflectionMethods.GetIndexerValue(req.Path[0]);
                    if (index == null) return;

                    var item = ReflectionMethods.GetItem(prop.Value, index.Value);
                    prop.Value = GetXenProperties(item);
                }
            }

            prop.Path = req.Path?.Union(prop.Path)?.ToArray();
            var cantCast = SetPath(prop.Value, req.Path);

            ctx.SetResponse<ObjectResponse>(res =>
            {
                res.UnknownCondition = cantCast;
                res.Property = prop;
                res.WidgetId = req.WidgetId;
                res.ObjectName = XenProperty.GetLastPath(req.Path);
            });
        }


        private bool SetPath(object value, params string[] path)
        {
            try
            {
                var properties = (XenProperty[])value;

                if (properties != null)
                {
                    foreach (var p in properties)
                    {
                        p.Path = path?.Union(p.Path)?.ToArray();
                    }
                }
            }
            catch (InvalidCastException)
            {
                return false;
            }

            return true;
        }
    }
}