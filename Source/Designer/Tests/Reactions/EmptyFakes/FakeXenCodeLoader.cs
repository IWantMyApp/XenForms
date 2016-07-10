using System;
using System.Reflection;
using XenForms.Core.Reflection;

namespace XenForms.Designer.Tests.Reactions.EmptyFakes
{
    public class FakeXenCodeLoader : IXenCodeLoader
    {
        public object CreateInstance(Type type)
        {
            throw new NotImplementedException();
        }

        public DesignAssembly GetAssemblyInformation(byte[] raw)
        {
            throw new NotImplementedException();
        }

        public Assembly GetAssembly(string fullName)
        {
            throw new NotImplementedException();
        }

        public DesignAssembly Load(byte[] raw, out Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public Type[] GetExportedTypes(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public MethodInfo[] GetExportedMethods(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public bool AttachEventHandler(object source, string eventName, object handler, MethodInfo method)
        {
            throw new NotImplementedException();
        }
    }
}
