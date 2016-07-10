using System;

namespace XenForms.Core.Toolbox
{
    [Flags]
    public enum PouchTypes
    {
        None = 0,
        Properties = 1,
        Events = 2,
        AttachedProperties = 4
    }
}
