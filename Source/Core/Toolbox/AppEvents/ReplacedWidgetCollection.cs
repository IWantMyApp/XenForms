using XenForms.Core.Widgets;

namespace XenForms.Core.Toolbox.AppEvents
{
    public class ReplacedWidgetCollection : IAppEvent
    {
        public XenWidget Widget { get; private set; }


        public ReplacedWidgetCollection(XenWidget widget)
        {
            Widget = widget;
        }
    }
}