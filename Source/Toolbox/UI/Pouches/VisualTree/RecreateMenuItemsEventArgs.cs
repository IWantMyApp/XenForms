using System;
using XenForms.Core.Toolbox.Project;

namespace XenForms.Toolbox.UI.Pouches.VisualTree
{
    public class RecreateMenuItemsEventArgs : EventArgs
    {
        public ProjectView[] Views { get; set; }

        public RecreateMenuItemsEventArgs(ProjectView[] views)
        {
            Views = views;
        }
    }
}