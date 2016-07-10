using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Reflection;
using XenForms.Core.Widgets;

namespace XenForms.Core.Designer.Reactions
{
    public class GetConstructorsReaction<TVisualElement> : DesignerReaction<TVisualElement>
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            if (ctx.Request == null) return;

            var req = ctx.Get<GetConstructorsRequest>();
            if (req == null) return;

            var type = TypeFinder.Find(req.TypeName);
            if (type == null) return;

            var ctors = XenConstructorMethods.GetConstructors(type);

            foreach (var ctor in ctors)
            {
                foreach (var p in ctor.Parameters)
                {
                    var pType = TypeFinder.Find(p.TypeName);

                    p.XenType = new XenType
                    {
                        Descriptor = Descriptor.GetDescriptors(pType),
                        FullName = pType.FullName,
                    };

                    Descriptor.SetPossibleValues(pType, p.XenType);
                }
            }

            ctx.SetResponse<GetConstructorsResponse>(res =>
            {
                var descs = Descriptor.GetDescriptors(type);

                res.Type = new XenType
                {
                    FullName = req.TypeName,
                    Descriptor = descs,
                    Constructors = ctors
                };
            });
        }
    }
}