using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Widgets;

namespace XenForms.Core.Designer.Reactions
{
    /// <summary>
    /// An action that will return all <see cref="XenWidget"/>s on the design surface.
    /// </summary>
    /// <typeparam name="TVisualElement"></typeparam>
    public class GetVisualTreeReaction<TVisualElement> : DesignerReaction<TVisualElement>
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            var r = XenMessage.Create<GetVisualTreeResponse>();
            r.Root = Surface.Root;
            ctx.Response = r;
        }
    }
}