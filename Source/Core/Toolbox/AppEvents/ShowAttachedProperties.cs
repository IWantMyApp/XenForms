using XenForms.Core.Widgets;

namespace XenForms.Core.Toolbox.AppEvents
{
    public class ShowAttachedProperties : IAppEvent
    {
        public XenWidget Widget { get; set; }

        public ShowAttachedProperties(XenWidget widget)
        {
            Widget = widget;
        }
    }
}
