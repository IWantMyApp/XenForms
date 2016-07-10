using System;
using System.Linq;
using System.Reflection;
using XenForms.Core.Widgets;

namespace XenForms.Core.Reflection
{
    public static class XenPropertyMethods
    {
        public static XenProperty[] GetObjectProperties(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var type = value.GetType();

            var result = type
                .GetPublicProperties()
                .Select(p => new XenProperty
                {
                    XenType = new XenType
                    {
                        FullName = p.PropertyType.FullName,
                    },
                    Path = new[] { p.Name },
                    CanRead = p.CanRead,
                    CanWrite = p.CanWrite,
                    Value = p.GetValue(value),
                }).ToArray();

            return result;
        }


        /// <summary>
        /// Return primitive value types, such as byte, short, int, long.
        /// String and DateTime are not considered primitive value types.
        /// </summary>
        /// <param name="value">struct</param>
        public static XenProperty[] GetPrimitiveValueTypeProperties(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var type = value.GetType();
            XenProperty[] result = {};

            if (ReflectionMethods.IsNotPrimitiveValueType(value))
            {
                result = type
                    .GetPublicProperties()
                    .Where(p =>
                    {
                        var pi = p.PropertyType.GetTypeInfo();
                        if (pi == null) return false;
                        return (pi.IsPrimitive && pi.IsValueType) || pi.IsEnum;
                    })
                    .Select(p => new XenProperty
                    {
                        XenType = new XenType
                        {
                            FullName = p.PropertyType.FullName,
                        },
                        Path = new[] {p.Name},
                        CanRead = p.CanRead,
                        CanWrite = p.CanWrite,
                        Value = p.GetValue(value),
                    })
                    .ToArray();
            }

            return result;
        }
    }
}