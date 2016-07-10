using XenForms.Core.Reflection;

namespace XenForms.Core.Messages
{
    public class AttachEventHandlerRequest : Request, IWidgetMessage
    {
        public DesignAssembly GeneratedAssembly { get; set; }
        public string EventName { get; set; }
        public string WidgetId { get; set; }
    }
}