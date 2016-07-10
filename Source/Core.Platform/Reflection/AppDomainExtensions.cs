using System;
using System.Linq;
using XenForms.Core.Reflection;

namespace XenForms.Core.Platform.Reflection
{
    public static class AppDomainExtensions
    {
        public static Type[] GetNewableTypesWithAttribute<T>(this AppDomain ad)
        {
            var types = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(ReflectionMethods.IsXenFormsDiscoverable)
                .SelectMany(s => s.GetTypes())
                .Where(t => Attribute.IsDefined(t, typeof(T)))
                .Where(t => !t.IsAbstract && t.IsPublic && !t.IsInterface)
                .ToArray();

            return types;
        }
    }
}