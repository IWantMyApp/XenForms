using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XenForms.Core.XAML
{
    public class XamlElement
    {
        public XamlElement()
        {
            Attributes = new Dictionary<XName, string>(5);
        }

        public XName Name { get; set; }
        public string NodeName { get; set; }
        public Dictionary<XName,string> Attributes { get; set; }

        public string GetAttributeValue(string localName)
        {
            var match = Attributes.FirstOrDefault(a => a.Key.LocalName == localName);
            return match.Value;
        }
    }


    public class XamlTreeReader
    {
        private readonly string _xml;
        private XDocument _doc;
        private XElement[] _all;

        public bool IsDocumentLoaded => _doc?.Document != null;
        public XamlElement[] All { get; private set; }


        public XamlTreeReader(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) throw new ArgumentNullException(nameof(xml));

            _xml = xml;
            _all = new XElement[]{};
            All = new XamlElement[] {};
        }


        public bool Read()
        {
            _doc = XDocument.Parse(_xml, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);

            _all = _doc?.Document?.Descendants().ToArray();
            if (_all == null) return false;

            var els = new List<XamlElement>();
            foreach (var e in _all)
            {
                var attribs = e.Attributes().ToDictionary(a => a.Name, a => a.Value);

                var xe = new XamlElement
                {
                    Name = e.Name,
                    NodeName = e.Name.LocalName,
                    Attributes = attribs
                };

                els.Add(xe);
            }

            All = els.ToArray();

            return true;
        }
    }
}