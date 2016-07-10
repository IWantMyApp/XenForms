using System;
using System.Reflection;

namespace XenForms.Core.Reflection
{
    public interface IXenCodeLoader
    {
        object CreateInstance(Type type);
        bool AttachEventHandler(object source, string eventName, object handler, MethodInfo method);
        MethodInfo[] GetExportedMethods(Assembly assembly);
        Type[] GetExportedTypes(Assembly assembly);
        DesignAssembly Load(byte[] raw, out Assembly assembly);
        DesignAssembly GetAssemblyInformation(byte[] raw);
        Assembly GetAssembly(string fullName);
    }
}