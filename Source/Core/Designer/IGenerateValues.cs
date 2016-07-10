using System;

namespace XenForms.Core.Designer
{
    public interface IGenerateValues
    {
        string[] Get(Type type);
    }
}