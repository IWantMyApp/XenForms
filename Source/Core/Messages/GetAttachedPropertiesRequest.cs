using XenForms.Core.Widgets;

namespace XenForms.Core.Messages
{
    public class GetAttachedPropertiesRequest : Request
    {
        public string WidgetId { get; set; }
    }

    public class GetAttachedPropertiesResponse : Response
    {
        public XenWidget Widget { get; set; }
    }
}