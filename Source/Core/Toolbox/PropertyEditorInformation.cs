using System.Diagnostics;

namespace XenForms.Core.Toolbox
{
    [DebuggerDisplay("Designer: {DesignTypeName}; Toolbox: {ToolboxTypeName}")]
    public class PropertyEditorInformation
    {
        public string DesignTypeName { get; set; }
        public string ToolboxTypeName { get; set; }
    }
}
