using XenForms.Core.Widgets;

namespace XenForms.Core.Messages
{
    public class ObjectResponse : Response, IWidgetMessage
    {
        public string WidgetId { get; set; }
        public string ObjectName { get; set; }
        public XenProperty Property { get; set; }
        public bool UnknownCondition { get; set; }
    }
}