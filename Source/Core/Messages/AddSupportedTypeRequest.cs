using XenForms.Core.Widgets;

namespace XenForms.Core.Messages
{
    public class AddSupportedTypeRequest : Request
    {
        public XenType Type { get; set; }
    }

    public class AddSupportedTypeResponse : Response
    {
        public bool Added { get; set; }
        public bool AlreadyRegistered { get; set; }
        public string DisplayMessage { get; set; }
    }
}