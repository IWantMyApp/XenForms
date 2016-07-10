// ReSharper disable ClassNeverInstantiated.Local

using NUnit.Framework;
using XenForms.Core.Messages;
using XenForms.Core.Platform.Reflection;

namespace XenForms.Designer.Tests.Reflection
{
    [TestFixture]
    public class TestXenMessageFinder
    {
        public class TestXenMessage : XenMessage { }
        public class TestClass { }


        [Test]
        public void Find_class_that_inherits_XenMessage()
        {
            var finder = new XenMessageFinder();
            var result = finder.Find(nameof(TestXenMessage));
            Assert.IsNotNull(result);
        }


        [Test]
        public void Return_null_for_non_XenMessages()
        {
            var finder = new XenMessageFinder();
            var result = finder.Find(nameof(TestClass));
            Assert.IsNull(result);
        }


        [Test]
        public void Return_null_for_type_that_doesnt_exist()
        {
            var finder = new XenMessageFinder();
            var doesntExist = finder.Find("something_123");
            Assert.IsNull(doesntExist);

            var nullPassed = finder.Find(null);
            Assert.IsNull(nullPassed);
        }
    }
}