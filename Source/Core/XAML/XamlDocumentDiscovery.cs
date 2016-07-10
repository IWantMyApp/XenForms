using System;
using System.Linq;
using System.Xml.Linq;

namespace XenForms.Core.XAML
{
    public class XamlDocumentDiscovery
    {
        private readonly XDocument _doc;
        public static XName XClass;

        static XamlDocumentDiscovery()
        {
            XClass = XName.Get("Class", "http://schemas.microsoft.com/winfx/2009/xaml");
        }

        public XamlDocumentDiscovery(string xaml)
        {
            if (string.IsNullOrWhiteSpace(xaml)) throw new ArgumentNullException(nameof(xaml));
            _doc = XDocument.Parse(xaml, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
        }


        public string GetPageClassName()
        {
            var page = _doc.Document?.Elements().FirstOrDefault();
            var xClassName = page?.Attributes(XClass).FirstOrDefault();
            return xClassName?.Value;
        }
    }
}