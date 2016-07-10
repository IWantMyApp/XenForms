using XenForms.Core.Messages;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Widgets;

namespace XenForms.Core.Toolbox.Actions
{
    /// <summary>
    /// Handle a <see cref="GetVisualTreeResponse"/> from the design server.
    /// </summary>
    public class GetVisualTreeAction : ToolboxAction
    {
        protected override void OnExecute(XenMessage message)
        {
            var response = message as GetVisualTreeResponse;

            if (response != null)
            {
                VisualTree.Root = new VisualTreeNode
                {
                    Widget = response.Root
                };

                SetParent(VisualTree.Root.Widget);
                Bus.Notify(new NewVisualTreeRootSet());
            }
        }


        private void SetParent(XenWidget widget)
        {
            var parent = widget;

            foreach (var child in widget.Children)
            {
                child.Parent = parent;
                SetParent(child);
            }
        }
    }
}
