using System;

namespace XenForms.Core.Toolbox
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PropertyEditorAttribute : Attribute
    {
        public string DesignTypeName { get; set; }
        public string ToolboxTypeName { get; set; }


        public PropertyEditorAttribute(Type designType, Type toolboxType) : this(designType.FullName, toolboxType.FullName) { }
        public PropertyEditorAttribute(Type designType) : this(designType.FullName, designType.FullName) { }
        public PropertyEditorAttribute(string designType, Type toolboxType) : this(designType, toolboxType.AssemblyQualifiedName) { }


        public PropertyEditorAttribute(string designTypeName, string toolboxTypeName)
        {
            if (string.IsNullOrWhiteSpace(designTypeName)) { throw new ArgumentNullException(nameof(designTypeName));}
            if (string.IsNullOrWhiteSpace(toolboxTypeName)) { throw new ArgumentNullException(nameof(toolboxTypeName));}

            DesignTypeName = designTypeName;
            ToolboxTypeName = toolboxTypeName;
        }
    }
}
