using System;
using XenForms.Core.Widgets;

namespace XenForms.Core.Toolbox
{
    public class PropertyEditorSavingEventArgs : EventArgs
    {
        public VisualTreeNode Node { get; set; }
        public XenProperty Property { get; set; }
        public object ToolboxValue { get; set; }
        public bool IsBase64 { get; set; }
        public object Metadata { get; set; }
    }
}