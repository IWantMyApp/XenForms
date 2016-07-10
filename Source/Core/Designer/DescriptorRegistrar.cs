using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XenForms.Core.Designer.Generators;
using XenForms.Core.Reflection;
using XenForms.Core.Widgets;

namespace XenForms.Core.Designer
{
    public class DescriptorRegistrar
    {
        #region Singleton


        private DescriptorRegistrar() {}


        private static DescriptorRegistrar _registrar;
        public static DescriptorRegistrar Create(ITypeFinder typeFinder)
        {
            if (_registrar == null)
            {
                _registrar = new DescriptorRegistrar
                {
                    _typeFinder = typeFinder
                };
            }

            return _registrar;
        }


        #endregion


        private readonly Dictionary<Type, Item> _items = new Dictionary<Type, Item>();
        private ITypeFinder _typeFinder;


        private class Item
        {
            public Type Type { get; set; }
            public XenPropertyDescriptors Descriptor { get; set; }
            public IGenerateValues Generator { get; set; }
        }


        public int Count => _items.Count;


        public void Reset()
        {
            _items.Clear();
        }


        public void Add(Type type, IGenerateValues generator)
        {
            Add(type, XenPropertyDescriptors.None, generator);
        }


        public void Add(Type type, XenPropertyDescriptors descriptor, IGenerateValues generator = null)
        {
            var value = new Item
            {
                Type = type,
                Descriptor = descriptor,
                Generator = generator
            };

            _items.Add(type, value);
        }


        public void SetPossibleValues(Type type, XenType xenType)
        {
            var entry = GetItem(type);

            xenType.PossibleValues = entry.Generator.Get(type);

            if (xenType.PossibleValues?.Length > 0)
            {
                xenType.Descriptor |= XenPropertyDescriptors.Literals;
            }
        }


        public void SetPossibleValues(XenProperty property)
        {
            if (property.Value == null) return;

            property.XenType.Descriptor = GetDescriptors(property.Value.GetType());

            var entry = GetItem(property.Value.GetType());
            if (entry?.Generator == null) return;

            var type = _typeFinder.Find(property.XenType.FullName);
            if (type == null) return;

            property.XenType.PossibleValues = entry.Generator.Get(type);

            if (property.XenType.PossibleValues?.Length > 0)
            {
                property.XenType.Descriptor |= XenPropertyDescriptors.Literals;
            }
        }


        public void SetPossibleValues(XenReflectionProperty xRef, XenProperty xProp)
        {
            if (xRef == null || xProp == null) return;

            xProp.XenType = CreateType(xRef);

            var entry = GetItem(xProp.Value?.GetType());
            if (entry?.Generator == null) return;

            var type = _typeFinder.Find(xProp.XenType.FullName);
            if (type == null) return;

            xProp.XenType.PossibleValues = entry.Generator.Get(type);

            if (xProp.XenType.PossibleValues?.Length > 0)
            {
                xProp.XenType.Descriptor |= XenPropertyDescriptors.Literals;
            }
        }


        public void SetPossibleValues(XenReflectionProperty xRef, XenProperty xProp, Enum e)
        {
            if (xRef == null || xProp == null) return;

            xProp.XenType = CreateType(xRef);
            var item = GetItem(xProp.XenType.GetType());

            var gen = new EnumGenerator();
            var result = gen.Get(e.GetType());

            if (item != null)
            {
                xProp.XenType.Descriptor = item.Descriptor;
            }

            xProp.XenType.PossibleValues = result;
            xProp.XenType.Descriptor |= XenPropertyDescriptors.Literals;

            if (e.HasFlags())
            {
                xProp.XenType.Descriptor |= XenPropertyDescriptors.Flags;
            }
        }


        internal XenPropertyDescriptors GetDescriptors(Type type)
        {
            var entry = GetItem(type);
            if (entry == null) return XenPropertyDescriptors.None;

            return entry.Descriptor;
        }


        private Item GetItem(Type type)
        {
            if (type == null) return null;

            var match = _items
                .Where(e => e.Value.Type == type)
                .Select(e => (KeyValuePair<Type, Item>?)e)
                .FirstOrDefault();

            if (match == null)
            {
                if (type.GetTypeInfo().IsValueType)
                {
                    var valueTypeGenerator = _items
                        .Where(e =>
                        {
                            return e.Value.Type == typeof (ValueType);
                        })
                        .Select(e => (KeyValuePair<Type, Item>?)e)
                        .FirstOrDefault();

                    return valueTypeGenerator?.Value;
                }
            }

            return match?.Value;
        }


        public XenType CreateType(MethodInfo info)
        {
            return new XenType
            {
                Descriptor = GetDescriptors(info.ReturnType),
                FullName = info.ReturnType.FullName
            };
        }


        public XenType CreateType(FieldInfo fieldInfo)
        {
            return new XenType
            {
                Descriptor = GetDescriptors(fieldInfo.FieldType),
                FullName = fieldInfo.FieldType.FullName
            };
        }


        public XenType CreateType(XenReflectionProperty xRef)
        {
            return new XenType
            {
                Descriptor = GetDescriptors(xRef.TargetType),
                FullName = xRef.TargetType.FullName,
            };
        }
    }
}