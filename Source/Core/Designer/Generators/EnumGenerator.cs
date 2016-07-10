using System;

namespace XenForms.Core.Designer.Generators
{
    public class EnumGenerator : IGenerateValues
    {
        public string[] Get(Type t)
        {
            return t == null ? null : Enum.GetNames(t);
        }
    }
}