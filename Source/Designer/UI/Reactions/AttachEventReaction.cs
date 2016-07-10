using XenForms.Core.Messages;
using XenForms.Core.Networking;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    public class AttachEventReaction : XamarinFormsReaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            var request = ctx.Get<AttachEventHandlerRequest>();
            if (request == null) return;

            /*
            var target = Surface[request.WidgetId];
            if (target == null) return;

            var assembly = Loader.GetAssembly(request.GeneratedAssembly.FullName);
            var types = Loader.GetExportedTypes(assembly);
            var obj = Loader.CreateInstance(types[0]);
            var methods = Loader.GetExportedMethods(assembly);

            Loader.AttachEventHandler(target.VisualElement, request.EventName, obj, methods[0]);
            */
        }
    }
}
