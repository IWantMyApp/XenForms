using NUnit.Framework;
using XenForms.Core.Designer;
using XenForms.Core.Platform.Reflection;
using XenForms.Core.Reflection;
using XenForms.Core.Widgets;
using XenForms.Designer.Tests.EmptyFakes;

namespace XenForms.Designer.Tests.Designer
{
    [TestFixture]
    public class TestDescriptorRegistrar
    {
        private DescriptorRegistrar Dr { get; set; }


        [SetUp]
        public void BeforeTest()
        {
            Dr = DescriptorRegistrar.Create(new TypeFinder());
            Dr.Reset();
        }


        [Test]
        public void Test_flags_and_staticlist()
        {
            Dr.Add(typeof(FakeEnumWithFlags), XenPropertyDescriptors.None);

            var t1 = FakeEnumWithFlags.Test1;

            var xRef = new XenReflectionProperty
            {
                TargetType = typeof (FakeEnumWithFlags),
            };

            var xProp = new XenProperty
            {
                Value = t1
            };

            Dr.SetPossibleValues(xRef, xProp, t1);

            Assert.IsTrue(xProp.XenType.Descriptor.HasFlag(XenPropertyDescriptors.Flags | XenPropertyDescriptors.Literals));
        }


        [Test]
        public void Test_Add_using_XenPropertyDescriptor()
        {
            Dr.Add(typeof (FakeImage), XenPropertyDescriptors.Image);
            var usingType = Dr.GetDescriptors(typeof (FakeImage));
            Assert.AreEqual(usingType, XenPropertyDescriptors.Image);
        }


        [Test]
        public void Test_Add_using_Generator()
        {
            Dr.Add(typeof(FakeImage), XenPropertyDescriptors.Image, new FakeImageGenerator());
        }
    }
}
