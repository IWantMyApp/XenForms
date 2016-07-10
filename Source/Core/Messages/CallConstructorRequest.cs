using XenForms.Core.Widgets;

namespace XenForms.Core.Messages
{
    public class CallConstructorRequest : Request
    {
        public XenConstructor Constructor { get; set; }
        public XenProperty Property { get; set; }
        public string WidgetId { get; set; }
    }

    public class CallConstructorResponse : Response
    {
        public string ErrorMessage { get; set; }
        public bool Successful { get; set; }
    }
}