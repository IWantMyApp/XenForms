using System;
using XenForms.Core.Designer;

namespace XenForms.Designer.Tests.Reactions.EmptyFakes
{
    public class FakeDesignThread : IDesignThread
    {
        public object Context { get; set; }
        public void Invoke(Action action)
        {
            action();
        }
    }
}
