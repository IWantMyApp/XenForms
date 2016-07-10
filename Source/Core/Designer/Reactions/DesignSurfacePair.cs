using XenForms.Core.Widgets;

namespace XenForms.Core.Designer.Reactions
{
    /// <summary>
    /// Associate a <see cref="XenWidget"/> with the target framework's <see cref="VisualElement"/>.
    /// </summary>
    /// <typeparam name="TVisualElement"></typeparam>
    public class DesignSurfacePair<TVisualElement>
    {
        /// <summary>
        /// The XenForms' representation of the target framework's visual elemet. i.e. entry, grid, etc.
        /// </summary>
        public XenWidget XenWidget { get; set; }


        /// <summary>
        /// The target framework's visual element, i.e. view.
        /// </summary>
        public TVisualElement VisualElement { get; set; }
    }
}