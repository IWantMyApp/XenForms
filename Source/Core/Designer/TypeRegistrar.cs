using System;
using System.Collections.Generic;
using System.Linq;
using XenForms.Core.Reflection;
using XenForms.Core.Widgets;

namespace XenForms.Core.Designer
{
    public class TypeRegistrar
    {
        private TypeRegistrar()
        {
            Types = new HashSet<XenType>(new XenTypeComparer());
        }


        private static TypeRegistrar _instance;
        public static TypeRegistrar Instance => _instance ?? (_instance = new TypeRegistrar());

        public HashSet<XenType> Types { get; internal set; }


        public bool IsRegistered(XenType type)
        {
            return Types.Contains(type);
        }
        

        public bool IsRegistered(XenReflectionProperty prop, RegistrarMatches matches)
        {
            var nameMatch = Types
                .Any(t => t.FullName != null && t.FullName.Equals(prop.TargetType.FullName, StringComparison.CurrentCultureIgnoreCase));

            if (matches.HasFlag(RegistrarMatches.TypeName | RegistrarMatches.Enum))
            {
                return nameMatch || prop.IsTargetEnum;
            }

            if (matches.HasFlag(RegistrarMatches.TypeName))
            {
                return nameMatch;
            }

            if (matches.HasFlag(RegistrarMatches.Enum))
            {
                return prop.IsTargetEnum;
            }

            return false;
        }


        public bool IsRegistered(Type type)
        {
            return Types.Any(t => t.FullName.Equals(type.FullName, StringComparison.CurrentCultureIgnoreCase));
        }


        public void SetTypes(params Type[] types)
        {
            Types = new HashSet<XenType>(types.Select(type => new XenType {FullName = type.FullName}));
        }


        public bool AddType(XenType type)
        {
            if (string.IsNullOrWhiteSpace(type?.FullName)) return false;

            return _instance.Types.Add(type);
        }
    }
}