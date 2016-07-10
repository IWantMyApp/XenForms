using NUnit.Framework;
using XenForms.Core.XAML;

namespace XenForms.Designer.Tests.XML
{
    [TestFixture]
    public class XamlDocumentDiscoveryTest
    {
        [Test]
        public void Should_find_Page_Type()
        {
            var xaml = XAMLTests.CustomPageType;
            var er = new XamlDocumentDiscovery(xaml);

            var pageName = er.GetPageClassName();
            Assert.AreEqual("Events.SliderAndButton", pageName);
        }
    }
}
