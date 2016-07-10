using System;
using XenForms.Core.Widgets;

namespace XenForms.Core.Toolbox
{
    public interface IPropertyEditorModel
    {
        /// <summary>
        /// When constructing an object, should we use the default value received from the designer?
        /// </summary>
        bool UseDefaultValue { get; set; }
        bool IsInitialized { get; }
        int UpdateCount { get; }

        Type ToolboxType { get; }

        object ToolboxValue { get; set; }
        object DisplayValue { get; }

        string DisplayName { get; set; }
        string FullTypeName { get; set; }
        string ShortTypeName { get; set; }

        bool IsNull { get; set; }
        bool SaveInBase64 { get; set; }

        VisualTreeNode VisualNode { get; set; }
        XenProperty Property { get; set; }
        event EventHandler<PropertyEditorSavingEventArgs> Saving;
    }
}
