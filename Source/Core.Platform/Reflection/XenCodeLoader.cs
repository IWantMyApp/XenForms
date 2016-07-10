using System;
using System.Linq;
using System.Reflection;
using XenForms.Core.Reflection;

namespace XenForms.Core.Platform.Reflection
{
    public class XenCodeLoader : IXenCodeLoader
    {
        public object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }


        public Assembly GetAssembly(string fullName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == fullName);
            return assembly;
        }


        public bool AttachEventHandler(object source, string eventName, object handler, MethodInfo method)
        {
            var eventSource = source.GetType().GetEvent(eventName);
            if (eventSource == null) return false;

            var methodDelegate = Delegate.CreateDelegate(eventSource.EventHandlerType, handler, method);
            eventSource.AddEventHandler(source, methodDelegate);

            return true;
        }


        public Type[] GetExportedTypes(Assembly assembly)
        {
            var types = assembly.ExportedTypes
                .Where(t => Attribute.IsDefined(t, typeof(XenFormsExport)))
                .Where(t => !t.IsAbstract && t.IsPublic && !t.IsInterface)
                .ToArray();

            return types;
        }


        public MethodInfo[] GetExportedMethods(Assembly assembly)
        {
            var methods = assembly.GetTypes()
                      .SelectMany(t => t.GetMethods())
                      .Where(m => m.GetCustomAttributes(typeof(XenFormsExport), false).Length > 0)
                      .ToArray();

            return methods;
        }


        public DesignAssembly Load(byte[] raw, out Assembly assembly)
        {
            var loaded = Assembly.Load(raw);
            assembly = loaded;

            var name = loaded.GetName();

            var result = new DesignAssembly
            {
                FullName = name.FullName,
                ShortName = name.Name
            };

            return result;
        }


        public DesignAssembly GetAssemblyInformation(byte[] raw)
        {
            var assembly = Assembly.ReflectionOnlyLoad(raw);
            var name = assembly.GetName();

            var result = new DesignAssembly
            {
                FullName = name.FullName,
                ShortName = name.Name
            };

            return result;
        }
    }
}
