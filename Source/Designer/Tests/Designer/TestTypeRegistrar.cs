using NUnit.Framework;
using XenForms.Core.Designer;
using XenForms.Core.Reflection;
using XenForms.Core.Widgets;

namespace XenForms.Designer.Tests.Designer
{
    [TestFixture]
    public class TestTypeRegistrar
    {
        public TypeRegistrar Tr => TypeRegistrar.Instance;


        [Test]
        public void Contains_using_XenReflectionProperty()
        {
            Tr.SetTypes(typeof(int));

            var xProp = new XenReflectionProperty
            {
                TargetType = typeof (int)
            };

            var usingTypeName = Tr.IsRegistered(xProp, RegistrarMatches.TypeName);
            var usingEnum = Tr.IsRegistered(xProp, RegistrarMatches.Enum);

            Assert.IsTrue(usingTypeName, "by TypeName");
            Assert.IsFalse(usingEnum, "by Enum");
        }


        [Test]
        public void Contains_using_type()
        {
            Tr.SetTypes(typeof(int));
            Assert.IsTrue(Tr.IsRegistered(typeof(int)));
        }
    }
}