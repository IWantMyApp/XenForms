using XenForms.Core.Widgets;

namespace XenForms.Core.Messages
{
    /// <summary>
    /// A request to assign a value to a <see cref="XenWidget"/>'s property.
    /// </summary>
    public class SetPropertyRequest : Request, IWidgetMessage
    {
        public bool IsBase64 { get; set; }

        /// <summary>
        /// <see cref="XenWidget.Id"/>
        /// </summary>
        public string WidgetId { get; set; }

        /// <summary>
        /// The widget's property value.
        /// </summary>
        public object Value { get; set; }
        public object Metadata { get; set; }

        /// <summary>
        /// The ancestoral path from the widget
        /// </summary>
        public string[] Path { get; set; }


        public bool IsAttachedProperty { get; set; }
    }
}