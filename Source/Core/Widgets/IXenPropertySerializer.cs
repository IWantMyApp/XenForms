using System;

namespace XenForms.Core.Widgets
{
    public interface IXenPropertySerializer
    {
        Type Type { get; }
        object Convert(string json);
    }
}