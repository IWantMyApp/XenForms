using XenForms.Core.Widgets;

namespace XenForms.Core.Toolbox.AppEvents
{
    public class ShowWidgetPropertyEditors : IAppEvent
    {
        public XenWidget Widget { get; private set; }

        public ShowWidgetPropertyEditors(XenWidget widget)
        {
            Widget = widget;
        }
    }
}
