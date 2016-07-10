using XenForms.Core.Widgets;

namespace XenForms.Core.Messages
{
    public class XamlResponse : Response
    {
        public XenWidget Widget { get; set; }
        public XenWidget Parent { get; set; }
        public string[] XamlDefaults { get; set; }
    }
}