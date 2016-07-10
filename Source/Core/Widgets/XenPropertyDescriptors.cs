using System;

namespace XenForms.Core.Widgets
{
    [Flags]
    public enum XenPropertyDescriptors
    {
        None = 0,
        Image = 1,
        Literals = 2,
        Flags = 4,
        Static = 8,
        ValueType = 16,
        Enumerable = 32,
        Collection = 64,
        List = 128,
        AttachedProperty = 256
    }
}