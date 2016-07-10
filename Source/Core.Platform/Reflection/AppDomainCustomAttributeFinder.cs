using System;
using System.Collections.Generic;
using System.Linq;
using XenForms.Core.Reflection;

namespace XenForms.Core.Platform.Reflection
{
    public class AppDomainCustomAttributeFinder<T> : IFindCustomAttributes<T> where T : Attribute
    {
        private TypeAttributeAssociation<T>[] _attributes = null;


        public TypeAttributeAssociation<T>[] FindAll()
        {
            if (_attributes != null)
            {
                return _attributes;
            }

            var results = new List<TypeAttributeAssociation<T>>();
            var types = AppDomain.CurrentDomain.GetNewableTypesWithAttribute<T>();

            foreach (var type in types)
            {
                var attribs = type
                    .GetCustomAttributes(typeof (T), true)
                    .Cast<T>()
                    .ToArray();

                var assoc = new TypeAttributeAssociation<T>
                {
                    DecoratedType = type,
                    Attributes = attribs
                };

                results.Add(assoc);
            }

            _attributes = results.ToArray();
            return _attributes;
        }
    }
}