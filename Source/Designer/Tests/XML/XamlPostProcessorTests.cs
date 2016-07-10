using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using XenForms.Core.Platform.XAML;

namespace XenForms.Designer.Tests.XML
{
    [TestFixture]
    public class XamlPostProcessorTests
    {
        private const string Uri = "http://www.test.com";


        [Test]
        public void Is_Xaml_Schema()
        {
            var x1 = XName.Get("test1");
            var x2 = XName.Get("test2", Uri);
            var x3 = XName.Get("test3", XamlPostProcessor.Xaml2006Uri);
            var x4 = XName.Get("test4", XamlPostProcessor.Xaml2009Uri);

            Assert.IsFalse(XamlPostProcessor.IsXamlSchema(x1));
            Assert.IsFalse(XamlPostProcessor.IsXamlSchema(x2));
            Assert.IsTrue(XamlPostProcessor.IsXamlSchema(x3));
            Assert.IsTrue(XamlPostProcessor.IsXamlSchema(x4));
        }


        [Test]
        public void Document_is_loaded()
        {
            var proc = new XamlPostProcessor();
            proc.LoadDocument("<test></test>");
            Assert.IsTrue(proc.IsDocumentLoaded);
        }


        [Test]
        public void Document_is_not_loaded()
        {
            var proc = new XamlPostProcessor();
            Assert.IsFalse(proc.IsDocumentLoaded);
        }


        [Test]
        public void Document_is_not_loaded_after_reset()
        {
            var proc = new XamlPostProcessor();
            proc.LoadDocument("<test></test>");
            Assert.IsTrue(proc.IsDocumentLoaded);

            proc.Reset();

            Assert.IsFalse(proc.IsDocumentLoaded);
        }


        [Test]
        public void Should_not_load_empty_template()
        {
            var proc = new XamlPostProcessor();
            var loaded = proc.LoadElementDefaults(string.Empty);
            Assert.IsFalse(loaded);
        }


        [Test]
        public void Should_load_simple_template()
        {
            var proc = new XamlPostProcessor();
            var loaded = proc.LoadElementDefaults("<test a='b' />");
            Assert.IsTrue(loaded);

            var el = proc.GetElementDefaults(XName.Get("test"));
            Assert.IsTrue(el.Name == "test");
        }


        [Test]
        public void Should_not_load_simple_template_multiple_times()
        {
            const string tmpl = "<test a='b'></test>";
            var proc = new XamlPostProcessor();

            var attempt1 = proc.LoadElementDefaults(tmpl);
            Assert.IsTrue(attempt1);

            var attempt2 = proc.LoadElementDefaults(tmpl);
            Assert.IsFalse(attempt2);
        }


        [Test]
        public void Should_get_document_namespaces()
        {
            var proc = new XamlPostProcessor();
            proc.LoadDocument($"<root xmlns:a='{Uri}'></root>");
            var nss = proc.GetDocumentNamespaces();

            Assert.AreEqual(2, nss.Count);
            Assert.AreEqual(Uri, nss["a"]);
        }


        [Test]
        public void Should_determine_if_elements_are_loaded()
        {
            // the important distinction here is the namespace matching
            var proc = new XamlPostProcessor();
            proc.LoadElementDefaults("<test1 a='1' />");
            proc.LoadElementDefaults($"<test2 xmlns='{Uri}' a='2' />");

            Assert.IsTrue(proc.AreElementDefaultsLoaded("test1"));
            Assert.IsTrue(proc.AreElementDefaultsLoaded("test2", Uri));
            Assert.IsFalse(proc.AreElementDefaultsLoaded("test2"));
        }


        [Test]
        public void Should_strip_object_references_everywhere()
        {
            const string tmpl1 = "<tmpl1 Assoc='{x:Reference __ReferenceID1}' />";
            var e1 = XElement.Parse(tmpl1);

            const string tmpl2 = "<tmpl1 Assoc='{x:Reference __ReferenceID1}' Children='{x:Reference __ReferenceID1}' />";
            var e2 = XElement.Parse(tmpl2);

            const string tmpl3 = "<tmpl0 Assoc='{x:Reference __ReferenceID1}'><tmpl1 Assoc='{x:Reference __ReferenceID1}' /></tmpl0>";
            var e3 = XElement.Parse(tmpl3);

            const string tmpl4 = "<tmpl0 Assoc='{x:Reference __ReferenceID1}'><Reference>__ReferenceID1</Reference></tmpl0>";
            var e4 = XElement.Parse(tmpl4);

            const string tmpl5 = "<tmpl0 Parent='{x:Reference Obj2}'><Reference>__ReferenceID1</Reference></tmpl0>";
            var e5 = XElement.Parse(tmpl5);

            string tmpl6 = $"<Grid.Parent xmlns:x='{Uri}'><x:Reference>__ReferenceID0</x:Reference></Grid.Parent>";
            var e6 = XElement.Parse(tmpl6);

            var proc = new XamlPostProcessor();
            proc.StripObjectReferences(e1);
            proc.StripObjectReferences(e2);
            proc.StripObjectReferences(e3);
            proc.StripObjectReferences(e4);
            proc.StripObjectReferences(e5);
            proc.StripObjectReferences(e6);

            Assert.IsFalse(e1.HasAttributes);
            Assert.IsFalse(e2.HasAttributes);
            Assert.IsFalse(e3.HasAttributes);
            Assert.IsFalse((e3.LastNode as XElement).HasAttributes);

            Assert.IsFalse(e4.HasAttributes);
            Assert.IsNull(e4.LastNode);

            Assert.IsFalse(e5.HasAttributes);
            Assert.IsTrue(e6.IsEmpty);
        }


        [Test]
        public void Should_strip_multiple_object_references_from_document()
        {
            const string tmpl1 = "<tmpl0 Parent='{x:Reference __ReferenceID1}'><Reference>__ReferenceID1</Reference></tmpl0>";
            var doc1 = XDocument.Parse(tmpl1);
            var proc = new XamlPostProcessor();

            var ran = proc.ForAllElements(doc1.Descendants(), e => proc.StripObjectReferences(e));
            Assert.IsTrue(ran);

            var root = doc1.Document.Root;

            Assert.IsFalse(root.HasAttributes);
            Assert.IsTrue(root.IsEmpty);
        }


        [Test]
        public void Should_strip_empty_attached_properties()
        {
            const string tmpl1 = "<Grid><Grid.Parent></Grid.Parent></Grid>";
            var e1 = XElement.Parse(tmpl1);

            var proc = new XamlPostProcessor();
            proc.StripEmptyAttachedProperties(e1);

            Assert.IsTrue(e1.IsEmpty);
        }


        [Test]
        public void Should_rename_design_surface_elements()
        {
            var proc = new XamlPostProcessor();
            var doc = proc.LoadDocument(XAMLTests.DesignSurfaceElement);
            var all = doc.Document?.Root?.DescendantsAndSelf().ToArray();

            proc.RenameDesignSurfaceElements();
            Assert.AreEqual(XamlPostProcessor.ContentPageElement, doc.Document?.Root?.Name.LocalName);

            var parent = all.Any(e => e.Name.LocalName == "ContentPage.Parent");
            Assert.IsTrue(parent, "Parent");

            var other = all.Any(e => e.Name.LocalName == "Other");
            Assert.IsTrue(other, "Other");
        }
    }
}
