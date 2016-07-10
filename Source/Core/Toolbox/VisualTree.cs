using XenForms.Core.XAML;

namespace XenForms.Core.Toolbox
{
    public static class VisualTree
    {
        public static VisualTreeNode Root { get; set; }
        public static XamlElement[] XamlElements { get; set; }

        public static void Reset()
        {
            Root = null;
            XamlElements = null;
        }
    }
}