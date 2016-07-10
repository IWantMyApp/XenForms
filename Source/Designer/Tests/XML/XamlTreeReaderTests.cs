using System.Linq;
using NUnit.Framework;
using XenForms.Core.XAML;

namespace XenForms.Designer.Tests.XML
{
    [TestFixture]
    public class XamlTreeReaderTests
    {
        [Test]
        public void Should_find_elements()
        {
            var xtr = new XamlTreeReader(XAMLTests.TreeReader1);
            xtr.Read();

            Assert.IsTrue(xtr.IsDocumentLoaded);
            Assert.AreEqual(3, xtr.All.Length);

            Assert.AreEqual("DesignSurfacePage", xtr.All[0].NodeName);
            Assert.AreEqual(2, xtr.All[0].Attributes.Count);

            var entry = xtr.All.First(a => a.NodeName == "Entry");
            var fontSize = entry.GetAttributeValue("FontSize");

            Assert.AreEqual("18", fontSize);
        }
    }
}
