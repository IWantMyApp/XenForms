using System;
using XenForms.Core.Designer;

namespace XenForms.Designer.Tests.EmptyFakes
{
    public class FakeImageGenerator : IGenerateValues
    {
        public string[] Get(Type type)
        {
            return new[]
            {
                "Test1",
                "Test2"
            };
        }
    }
}