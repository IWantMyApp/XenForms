using System;

namespace XenForms.Core.Designer
{
    public interface IDesignThread
    {
        object Context { get; set; }
        void Invoke(Action action);
    }
}