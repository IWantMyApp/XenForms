using System;
using System.Collections.Generic;
using System.Linq;
using XenForms.Core.Reflection;
using XenForms.Core.Widgets;

namespace XenForms.Core.Toolbox
{
    public sealed class DefaultPropertyEditorManager : PropertyEditorManager
    {
        private readonly ITypeFinder _typeFinder;
        private readonly TypeAttributeAssociation<PropertyEditorAttribute>[] _associations;
        private List<PropertyEditorInformation> _typeEditors;


        public DefaultPropertyEditorManager(ITypeFinder typeFinder, IFindCustomAttributes<PropertyEditorAttribute> attributeFinder)
        {
            _typeFinder = typeFinder;
            _associations = attributeFinder.FindAll();
        }


        internal override List<PropertyEditorInformation> PropertyEditors
        {
            get
            {
                if (_typeEditors == null)
                {
                    Initialize();
                }

                return _typeEditors;
            }
            set
            {
                _typeEditors = value;
            }
        }


        internal override void Initialize()
        {
            SetTypeEditors(out _typeEditors);
        }


        internal override void Reset()
        {
            _typeEditors = null;
        }


        internal override IPropertyEditorModel Create(XenProperty property)
        {
            var info = GetTypeEditorInfo(property.XenType);
            if (info == null) return null;

            var model = CreateTypeEditorModel(info);

            if (model != null)
            {
                model.UseDefaultValue = true;
                model.Property = property;
                model.ToolboxValue = property.Value;
                model.FullTypeName = property.XenType.FullName;
                model.ShortTypeName = property.XenType.ShortName;

                if (model.Property.XenType.Descriptor.HasFlag(XenPropertyDescriptors.AttachedProperty))
                {
                    model.DisplayName = property.XamlPropertyName;
                }
                else
                {
                    model.DisplayName = property.PropertyName;
                }
            }

            return model;
        }


        internal override IPropertyEditorModel Create(XenParameter p)
        {
            var info = GetTypeEditorInfo(p.XenType);
            if (info == null) return null;

            var model = CreateTypeEditorModel(info);

            if (model != null)
            {
                model.Property = new XenProperty
                {
                    XenType = p.XenType,
                    CanRead = true,
                    CanWrite = true,
                    Value = p.Value,
                    Path = new [] { p.ParameterName}
                };

                model.UseDefaultValue = false;
                model.DisplayName = p.ParameterName;
                model.ToolboxValue = p.Value;
                model.FullTypeName = p.TypeName;
                model.ShortTypeName = p.ShortTypeName;
            }

            return model;
        }


        private IPropertyEditorModel CreateTypeEditorModel(PropertyEditorInformation info)
        {
            // todo:should check if the toolbox type would like to use another type editor;
            // ie in the case of a guid registered to a string and not an object
            var toolboxType = _typeFinder.Find(info.ToolboxTypeName);
            var modelType = typeof (PropertyEditorModel<>);
            var genType = modelType.MakeGenericType(toolboxType);
            var model = Activator.CreateInstance(genType) as IPropertyEditorModel;
            return model;
        }


        private PropertyEditorInformation GetTypeEditorInfo(XenType xenType)
        {
            var match = PropertyEditors
                .FirstOrDefault(info =>
                {
                    var typeMatch = info.DesignTypeName == xenType.FullName;
                    if (typeMatch) return true;

                    var desc = xenType.Descriptor;
                    if (desc.HasFlag(XenPropertyDescriptors.Literals) && !desc.HasFlag(XenPropertyDescriptors.ValueType))
                    {
                        if (info.DesignTypeName.Equals(typeof(Enum).FullName))
                        {
                            return true;
                        }
                    }

                    return false;
                });

            return match;
        }


        private void SetTypeEditors(out List<PropertyEditorInformation> editors)
        {
            editors = (from decorated in _associations
                    from attribute in decorated.Attributes
                select new PropertyEditorInformation
                {
                    ToolboxTypeName = attribute.ToolboxTypeName,
                    DesignTypeName = attribute.DesignTypeName,
                }).ToList();
        }


        #region Add Methods


        public override void Add(string designTypeName, Type toolboxType)
        {
            var info = new PropertyEditorInformation
            {
                ToolboxTypeName = toolboxType.FullName,
                DesignTypeName = designTypeName,
            };

            PropertyEditors.Add(info);
        }


        public override void Add(Type designType)
        {
            var info = new PropertyEditorInformation
            {
                ToolboxTypeName = designType.FullName,
                DesignTypeName = designType.FullName,
            };

            PropertyEditors.Add(info);
        }


        public override void Add(Type designType, Type toolboxType)
        {
            var info = new PropertyEditorInformation
            {
                ToolboxTypeName = toolboxType.FullName,
                DesignTypeName = designType.FullName,
            };

            PropertyEditors.Add(info);
        }


        public override void Add(string designTypeName, string toolboxTypeName)
        {
            var info = new PropertyEditorInformation
            {
                ToolboxTypeName = toolboxTypeName,
                DesignTypeName = designTypeName,
            };

            PropertyEditors.Add(info);
        }


        #endregion
    }
}