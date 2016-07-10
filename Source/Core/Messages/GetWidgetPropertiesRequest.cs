using XenForms.Core.Widgets;

namespace XenForms.Core.Messages
{
    /// <summary>
    /// Signal's that the toolbox has requested a XenWidget's properties.
    /// Properties can be immutable or mutable.
    /// </summary>
    public class GetWidgetPropertiesRequest : Request, IWidgetMessage
    {
        /// <summary>
        /// Id of the requested widget's properties.
        /// </summary>
        public string WidgetId { get; set; }
        public bool IncludeValues { get; set; }
    }

    /// <summary>
    /// Result of requesting a widget's properties.
    /// Sent in response to a <see cref="GetWidgetPropertiesRequest"/>.
    /// </summary>
    public class GetWidgetPropertiesResponse : Response
    {
        /// <summary>
        /// XenForms' representation of the target framework's visual element.
        /// </summary>
        public XenWidget Widget { get; set; }


        /// <summary>
        /// <see cref="Widget"/>'s properties.
        /// </summary>
        public XenProperty[] Properties { get; set; }
    }
}