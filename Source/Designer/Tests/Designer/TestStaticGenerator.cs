using NUnit.Framework;
using Xamarin.Forms;
using XenForms.Core.Designer.Generators;

namespace XenForms.Designer.Tests.Designer
{
    [TestFixture]
    public class TestStaticGenerator
    {
        private struct NoFields
        {
            public string Test { get; set; }
            public string Test2 { get; set; }
            public static string Test3 = string.Empty;
            public const string Test4 = "testing";
            private static NoFields Test5 = new NoFields();
        }

        [Test]
        public void No_static_fields_should_be_found()
        {
            var gen = new StaticGenerator();
            var result = gen.Get(typeof (NoFields));

            Assert.AreEqual(0, result.Length);
        }


        [Test]
        public void Zero_should_be_returned_from_Rectangle_type()
        {
            var gen = new StaticGenerator();
            var result = gen.Get(typeof (Rectangle));

            Assert.AreEqual(result.Length, 1);
            Assert.AreEqual("Zero", result[0]);
        }


        [Test]
        public void Eight_fields_should_be_returned_from_LayoutOptions_type()
        {
            var gen = new StaticGenerator();
            var result = gen.Get(typeof (LayoutOptions));
            Assert.AreEqual(8, result.Length);
        }
    }
}
