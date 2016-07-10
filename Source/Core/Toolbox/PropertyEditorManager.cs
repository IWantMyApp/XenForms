using System;
using System.Collections.Generic;
using XenForms.Core.Widgets;

namespace XenForms.Core.Toolbox
{
    public abstract class PropertyEditorManager
    {
        internal abstract List<PropertyEditorInformation> PropertyEditors { get; set; }

        internal abstract void Initialize();
        internal abstract void Reset();

        internal abstract IPropertyEditorModel Create(XenProperty property);
        internal abstract IPropertyEditorModel Create(XenParameter property);

        public abstract void Add(string designTypeName, Type toolboxType);
        public abstract void Add(string designTypeName, string toolboxTypeName);
        public abstract void Add(Type designType);
        public abstract void Add(Type designType, Type toolboxType);
    }
}