using XenForms.Core.Widgets;

namespace XenForms.Core.Toolbox.AppEvents
{
    public class ShowWidgetEvents : IAppEvent
    {
        public XenWidget Widget { get; set; }


        public ShowWidgetEvents(XenWidget widget)
        {
            Widget = widget;
        }
    }
}
