using System;
using System.Collections.Generic;
using System.Linq;
using XenForms.Core.Messages;
using XenForms.Core.Reflection;

namespace XenForms.Core.Platform.Reflection
{
    public class XenMessageFinder : IXenMessageFinder
    {
        private static readonly Dictionary<string, Type> Types = new Dictionary<string, Type>();
        private static List<Type> _messageTypes;


        public Type Find(string typeName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(typeName))
                {
                    return null;
                }

                if (Types.ContainsKey(typeName))
                {
                    return Types[typeName];
                }

                var assemblyType = Type.GetType(typeName, false);

                if (assemblyType != null)
                {
                    if (!assemblyType.IsSubclassOf(typeof(XenMessage)))
                    {
                        assemblyType = null;
                    }

                    Types[typeName] = assemblyType;
                    return assemblyType;
                }

                if (_messageTypes == null)
                {
                    _messageTypes = AppDomain
                        .CurrentDomain
                        .GetAssemblies()
                        .Where(ReflectionMethods.IsXenFormsDiscoverable)
                        .SelectMany(a => a.GetTypes())
                        .Where(t => t.IsSubclassOf(typeof(XenMessage)))
                        .ToList();
                }

                var found = _messageTypes.FirstOrDefault(t => t.Name.Equals(typeName));

                if (found == null)
                {
                    Types[typeName] = null;
                    return null;
                }

                Types[typeName] = found;

                return found;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
