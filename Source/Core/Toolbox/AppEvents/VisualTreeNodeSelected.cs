namespace XenForms.Core.Toolbox.AppEvents
{
    public class VisualTreeNodeSelected : IAppEvent
    {
        public VisualTreeNode Node { get; set; }

        public VisualTreeNodeSelected() { }
        public VisualTreeNodeSelected(VisualTreeNode node) : this()
        {
            Node = node;
        }
    }
}