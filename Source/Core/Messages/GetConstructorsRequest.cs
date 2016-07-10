using XenForms.Core.Widgets;

namespace XenForms.Core.Messages
{
    public class GetConstructorsRequest : Request
    {
        public string TypeName { get; set; }
    }

    public class GetConstructorsResponse : Response
    {
        public XenType Type { get; set; }
    }
}