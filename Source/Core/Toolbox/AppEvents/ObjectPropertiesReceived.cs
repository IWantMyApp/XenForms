using XenForms.Core.Widgets;

namespace XenForms.Core.Toolbox.AppEvents
{
    public class ObjectPropertiesReceived : IAppEvent
    {
        public string WidgetName { get; private set; }
        public string WidgetId { get; private set; }
        public XenProperty[] Properties { get; private set; }

        public ObjectPropertiesReceived(XenProperty[] properties, string widgetId, string widgetName)
        {
            Properties = properties;
            WidgetId = widgetId;
            WidgetName = widgetName;
        }
    }
}