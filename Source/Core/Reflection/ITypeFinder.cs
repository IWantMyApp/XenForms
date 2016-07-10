using System;

namespace XenForms.Core.Reflection
{
    public interface ITypeFinder
    {
        Type Find(string typeName);
    }
}