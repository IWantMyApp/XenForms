using XenForms.Core.Designer.Reactions;
using XenForms.Core.Widgets;

namespace XenForms.Core.Messages
{
    /// <summary>
    /// Signal's that the toolbox has requested a tree structure of the UI elements on the design surface.
    /// This "document outline" is created by a <see cref="DesignSurfaceManager{TVisualElement}"/>.
    /// </summary>
    public class GetVisualTreeRequest : Request {}

    /// <summary>
    /// Sent in response to a <see cref="GetVisualTreeRequest"/>.
    /// </summary>
    public class GetVisualTreeResponse : Response
    {
        /// <summary>
        /// The visual tree's top-most widget.
        /// </summary>
        public XenWidget Root { get; set; }
    }
}
