using System;
using System.Linq;
using XenForms.Core.Reflection;

namespace XenForms.Core.Designer.Generators
{
    public class StaticGenerator : IGenerateValues
    {
        public string[] Get(Type type)
        {
            return type?.GetStaticFields().Select(p => p.Name).ToArray();
        }
    }
}
