using XenForms.Core.Widgets;

namespace XenForms.Core.Toolbox.AppEvents
{
    public class ConstructorsReceived : IAppEvent
    {
        public XenType Type { get; set; }

        public ConstructorsReceived(XenType type)
        {
            Type = type;
        }
    }
}