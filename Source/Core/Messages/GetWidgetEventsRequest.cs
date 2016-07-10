using XenForms.Core.Widgets;

namespace XenForms.Core.Messages
{
    /// <summary>
    /// The toolbox has requested that all events for the <see cref="XenWidget"/> should be returned.
    /// </summary>
    public class GetWidgetEventsRequest : Request, IWidgetMessage
    {
        /// <summary>
        /// The widget's <see cref="XenWidget.Id"/>.
        /// </summary>
        public string WidgetId { get; set; }
    }

    /// <summary>
    /// The public and protected events for the requested <see cref="XenWidget"/>.
    /// </summary>
    public class GetWidgetEventsResponse : Response, IWidgetMessage
    {
        /// <summary>
        /// The <see cref="XenWidget.Id"/>.
        /// </summary>
        public string WidgetId { get; set; }


        /// <summary>
        /// The widget's public and protected events.
        /// </summary>
        public XenEvent[] Events { get; set; }
    }
}