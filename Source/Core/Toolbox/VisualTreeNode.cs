using System.Linq;
using XenForms.Core.Widgets;

namespace XenForms.Core.Toolbox
{
    /// <summary>
    /// A visual tree node is any <see cref="XenWidget"/> that is displayed by the toolbox.
    /// </summary>
    public class VisualTreeNode
    {
        public string DisplayName => Widget.Type;
        public XenWidget Widget { get; set; }
        public bool IsRoot => Widget.Parent == null;

        /// <summary>
        /// Search the visual tree for a <see cref="XenWidget"/> match the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public XenWidget Find(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var widgets = Widget?.GetNodeAndDescendants().ToArray();

            if (widgets?.Length == 0)
            {
                return null;
            }

            var match = widgets.FirstOrDefault(w => w.Id.Equals(id));

            return match;
        }
    }
}