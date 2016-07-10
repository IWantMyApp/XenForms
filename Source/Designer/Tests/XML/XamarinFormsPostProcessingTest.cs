using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using XenForms.Core.Platform.XAML;
using XenForms.Core.XAML;

namespace XenForms.Designer.Tests.XML
{
    [TestFixture]
    public class XamarinFormsPostProcessingTest
    {
        [Test]
        public void Should_remove_all_default_attributes_and_elements()
        {
            var proc = new XamlPostProcessor();

            proc.LoadElementDefaults(XAMLTests.Button_Defaults);
            proc.LoadElementDefaults(XAMLTests.Button_Page_Defaults);
            proc.LoadDocument(XAMLTests.Button_Page);

            var doc = proc.Process();
            var root = GetRoot(doc);
            var e1 = root.Elements().FirstOrDefault();

            Assert.AreEqual("ContentPage", root.Name.LocalName);
            Assert.IsTrue(e1?.IsEmpty);
        }


        [Test]
        public void Should_only_detect_changes()
        {
            var proc = new XamlPostProcessor();

            proc.LoadElementDefaults(XAMLTests.Entry_Defaults);
            proc.LoadElementDefaults(XAMLTests.Entry_Page_Defaults);
            proc.LoadDocument(XAMLTests.Entry_Page);

            var doc = proc.Process();
            var root = GetRoot(doc);
            var entry = root.Elements().First();
            var attribs = entry.Attributes().ToArray();

            var isEnabled = GetAttribute(attribs, "IsEnabled");
            var margin = GetAttribute(attribs, "Margin");
            var placeholder = GetAttribute(attribs, "Placeholder");

            Assert.AreEqual(5, attribs.Length);

            Assert.AreEqual("False", isEnabled.Value);
            Assert.AreEqual("30", margin.Value);
            Assert.AreEqual("Test", placeholder.Value);
        }


        [Test]
        public void Property_values_and_property_element_values_are_same()
        {
            var proc = new XamlPostProcessor();

            proc.LoadElementDefaults(XAMLTests.Nulls_ContentPage_Defaults);
            proc.LoadElementDefaults(XAMLTests.Nulls_Label_Defaults);
            proc.LoadDocument(XAMLTests.Nulls_Input);

            var doc = proc.Process();
            var els = doc?.Document?.Root?.Elements().ToArray();

            Assert.AreEqual(1, els?.Length);
        }


        [Test]
        public void Should_retain_attached_property_in_original_and_not_in_modified_version()
        {
            var xtr = new XamlTreeReader(XAMLTests.AP_Test1_Page);
            xtr.Read();

            var proc = new XamlPostProcessor();
            proc.LoadElementDefaults(XAMLTests.AP_Test1_Entry_Defaults);
            proc.LoadElementDefaults(XAMLTests.AP_Test1_Page_Default);
            proc.LoadDocument(XAMLTests.AP_Test1_Page_Modified, xtr.All);

            var doc = proc.Process();
            var els = doc.Descendants().ToArray();

            var ap1 = els[1].Attributes().FirstOrDefault(a => a.Name.LocalName == "AP.Test1");
            var ap2 = els[2].Attributes().FirstOrDefault(a => a.Name.LocalName == "AP.Test1");

            Assert.AreEqual("165", ap1.Value);
            Assert.IsNull(ap2);
        }


        private XAttribute GetAttribute(IEnumerable<XAttribute> attribs, string localName)
        {
            return attribs.FirstOrDefault(a => a.Name.LocalName == localName);
        }


        private XElement GetRoot(XDocument doc)
        {
            return doc?.Document?.Root;
        }
    }
}
