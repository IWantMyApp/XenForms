using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using XenForms.Core.XAML;

namespace XenForms.Core.Platform.XAML
{
    public class XamlPostProcessor : IXamlPostProcessor
    {
        internal const string Xaml2006Uri = "http://schemas.microsoft.com/winfx/2006/xaml";
        internal const string Xaml2009Uri = "http://schemas.microsoft.com/winfx/2009/xaml";

        internal const string DesignSurfaceElement = "DesignSurfacePage";
        internal const string ContentPageElement = "ContentPage";
        internal const string DesignerClassId = "designer.xenforms.com";

        private XDocument _docXml;
        private readonly Dictionary<XName, XElement> _defaults;
        private XamlElement[] _originals;

        public bool IsDocumentLoaded => _docXml?.Root != null;
        public XElement Root => _docXml?.Document?.Root;
        public XElement[] All => Root.DescendantsAndSelf().ToArray();


        public XElement[] TopLevel
        {
            get
            {
                var root = _docXml.Document?.Root;
                var elements = root?.Elements().ToArray();
                return elements ?? new XElement[] {};
            }
        }


        public XamlPostProcessor()
        {
            _defaults = new Dictionary<XName, XElement>();
        }


        public XDocument LoadDocument(string xml, XamlElement[] originals = null)
        {
            _originals = originals;
            _docXml = XDocument.Parse(xml, LoadOptions.SetLineInfo);
            return _docXml;
        }


        public void Reset()
        {
            _defaults.Clear();
            _docXml = null;
        }


        public bool LoadElementDefaults(string xml)
        {
            if (String.IsNullOrWhiteSpace(xml)) return false;

            var element = XElement.Parse(xml, LoadOptions.SetBaseUri | LoadOptions.SetLineInfo);
            var xname = element.Name;

            if (String.IsNullOrWhiteSpace(xname.LocalName)) return false;
            
            return SetElementDefaults(xname, element);
        }


        internal XElement GetElementAttributeDifferences(XElement element)
        {
            var include = new List<XAttribute>();
            var xmlnamespaces = GetDocumentNamespaces();

            // ensure that a template was loaded
            if (element == null) return null;
            if (GetElementDefaults(element.Name) == null) return null;

            // get attributes of element
            var defaultElement = _defaults[element.Name];
            var defaultAttributes = defaultElement.Attributes().ToArray();

            // attributes in both elements with different values
            foreach (var defaultAttribute in defaultAttributes)
            {
                var attribute = element.Attribute(defaultAttribute.Name);
                if (attribute == null) continue;

                var localName = attribute.Name.ToString();
                var value = attribute.Value;

                if (xmlnamespaces != null && xmlnamespaces.ContainsKey(attribute.Name.LocalName))
                {
                    var xmlnamespace = xmlnamespaces[attribute.Name.LocalName];

                    if (xmlnamespace == value)
                    {
                        include.Add(attribute);
                    }
                }

                if (String.IsNullOrWhiteSpace(localName)) continue;

                if (value != defaultAttribute.Value)
                {
                    include.Add(attribute);
                }
            }

            // attributes not in template.
            foreach (var elementAttribute in element.Attributes())
            {
                var localName = elementAttribute?.Name.ToString();
                if (String.IsNullOrWhiteSpace(localName)) continue;

                var exists = defaultAttributes.Any(a => a.Name == localName);

                if (!exists)
                {
                    include.Add(elementAttribute);
                }
            }

            var result = new XElement(element.Name);
            result.Add(include);

            return result;
        }


        public XDocument Process()
        {
            if (!IsDocumentLoaded) return null;
            var defaults = GetElementDefaults(Root.Name);

            // pass : top level element matching
            foreach (var element in TopLevel)
            {
                // exact match
                var exact = defaults?.Elements().FirstOrDefault(e => e.Name == element.Name);

                if (exact != null)
                {
                    if (XNode.DeepEquals(exact, element))
                    {
                        element.Remove();
                    }
                }
            }

            // pass: stripping extra prop values
            foreach (var element in TopLevel)
            {
                StripObjectReferences(element);
                StripEmptyAttachedProperties(element);
                StripDesignerNodes(element);
            }

            // pass: attached properties
            foreach (var element in All)
            {
                var ln = element.Name.LocalName;
                var parts = ln.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2) continue;

                // remove empty elements
                if (element.IsEmpty)
                {
                    element.Remove();
                    continue;
                }

                // compare property elements
                var xname = XName.Get(parts[0], element.Name.NamespaceName);
                var edefaults = GetElementDefaults(xname);

                var amatch = edefaults?.Attributes().FirstOrDefault(a => a.Name.LocalName == parts[1]);

                if (amatch != null)
                {
                    var childel = element.FirstNode as XElement;

                    // remove nullable matches
                    if (AreNullable(amatch.Value, childel?.Name))
                    {
                        element.Remove();
                        continue;
                    }
                }
            }

            // pass: deep matching
            foreach (var element in All)
            {
                var differences = GetElementAttributeDifferences(element);
                if (differences == null) continue;
                element.ReplaceAttributes(differences.Attributes());
            }

            if (_originals != null)
            {
                // pass: change element value to attribute value, if possible
                foreach (var element in All)
                {
                    var i = GetPosition(element);
                    var o = GetOriginal(element.Name, i);

                    if (o != null)
                    {
                        foreach (var oa in o.Attributes)
                        {
                            var ea = element.Attributes(oa.Key).FirstOrDefault();
                            if (ea == null)
                            {
                                var ap = new XAttribute(oa.Key, oa.Value);
                                element.Add(ap);
                            }
                        }
                    }
                }
            }

            RenameDesignSurfaceElements();

            return _docXml;
        }


        internal void StripDesignerNodes(XElement element)
        {
            if (element == null) return;

            var elements = element
                .Descendants()
                .Where(e => e.Attributes().Any(a => a.Name.LocalName == "ClassId" && a.Value.Equals(DesignerClassId)))
                .ToArray();

            if (elements.Any())
            {
                var attribs = Root?
                    .Attributes()
                    .FirstOrDefault(a => a.Name.LocalName == "NavigationPage.BackButtonTitle" && a.Value.Equals(DesignerClassId));

                if (attribs != null)
                {
                    Root?.Attributes().Where(a => a.Name.LocalName.Contains("NavigationPage.")).Remove();
                }
            }

            foreach (var current in elements)
            {
                current.Remove();
            }
        }


        internal void RenameDesignSurfaceElements()
        {
            var elements = Root?.DescendantsAndSelf().ToArray();
            if (elements == null) return;

            foreach (var element in elements)
            {
                if (element.Name.LocalName == DesignSurfaceElement)
                {
                    if (Root != null)
                    {
                        element.Name = XName.Get(ContentPageElement, Root.Name.NamespaceName);
                    }
                }

                if (element.Name.LocalName.Contains($"{DesignSurfaceElement}."))
                {
                    var index = element.Name.LocalName.IndexOf(".", StringComparison.Ordinal) + 1;
                    var property = element.Name.LocalName.Substring(index);

                    element.Name = XName.Get($"{ContentPageElement}.{property}", element.Name.NamespaceName);
                }
            }
        }


        internal void StripEmptyAttachedProperties(XElement element)
        {
            var elements = element?.DescendantsAndSelf().ToArray();
            if (elements == null) return;

            foreach (var current in elements)
            {
                if (string.IsNullOrWhiteSpace(current.Name.LocalName)) return;

                if (current.Name.LocalName.Contains(".") && !current.HasElements)
                {
                    if (!current.HasAttributes)
                    {
                        if (current.Parent != null)
                        {
                            current.Remove();
                            continue;
                        }
                    }

                    var attribs = current.Attributes().ToArray();

                    if (attribs.Length == 1 && IsXamlSchema(attribs[0].Name))
                    {
                        if (current.Parent != null)
                        {
                            current.Remove();
                            continue;
                        }
                    }
                }
            }
        }


        internal void StripObjectReferences(XElement element)
        {
            const string xref = "x:Reference";
            const string refid = "__ReferenceID";

            var attributes = element?.DescendantsAndSelf().Attributes().ToArray();
            if (attributes == null) return;

            foreach (var attribute in attributes)
            {
                if (attribute.Value.Contains(refid))
                {
                    if (attribute.Parent != null)
                    {
                        attribute.Remove();
                    }
                }

                if (attribute.Name.LocalName == "Parent" && attribute.Value.Contains(xref))
                {
                    if (attribute.Parent != null)
                    {
                        attribute.Remove();
                    }
                }
            }

            element
                .Elements()
                .Where(e => e.Parent != null && e.Name.LocalName == "Reference" && e.Value.Contains(refid))
                .Remove();

            element.Elements().Where(e =>
            {
                if (e.IsEmpty) return false;
                if (e.Parent == null) return false;
                if (!e.Name.LocalName.Contains(".")) return false;

                var first = e.FirstNode as XElement;
                if (first == null) return false;

                if (!IsXamlSchema(first.Name)) return false;
                if (string.IsNullOrWhiteSpace(first.Value)) return false;

                return first.Value.Contains(refid);
            }).Remove();
        }


        internal bool ForAllElements(IEnumerable<XElement> elements, Action<XElement> action)
        {
            if (elements == null) return false;

            var enumed = elements.ToArray();
            if (!enumed.Any()) return false;

            foreach (var el in enumed)
            {
                action(el);
            }

            return true;
        }


        internal IDictionary<string, string> GetDocumentNamespaces()
        {
            var navigator = _docXml.CreateNavigator();
            navigator.MoveToFollowing(XPathNodeType.Element);
            var namespaces = navigator.GetNamespacesInScope(XmlNamespaceScope.All);
            if (namespaces == null) return null;

            if (namespaces.ContainsKey(String.Empty))
            {
                var defaultNamespace = namespaces[String.Empty];
                namespaces["xmlns"] = defaultNamespace;
                namespaces.Remove(String.Empty);
            }

            return namespaces;
        }


        internal bool AreElementDefaultsLoaded(string localName, string namespaceName = null)
        {
            if (String.IsNullOrWhiteSpace(localName)) return false;

            var xname = namespaceName == null
                ? XName.Get(localName)
                : XName.Get(localName, namespaceName);

            return GetElementDefaults(xname) != null;
        }


        internal XElement GetElementDefaults(XName name)
        {
            if (_defaults.ContainsKey(name))
            {
                return _defaults[name];
            }

            return null;
        }


        internal bool SetElementDefaults(XName name, XElement element)
        {
            if (GetElementDefaults(name) == null)
            {
                _defaults[name] = element;
                return true;
            }

            return false;
        }


        internal XamlElement GetOriginal(XName name, int occurence)
        {
            var filtered = _originals
                .Where(o => o.Name == name)
                .Skip(occurence - 1)
                .Take(1);

            return filtered.FirstOrDefault();
        }


        internal int GetPosition(XElement e)
        {
            var index = Array.IndexOf(All, e);
            if (index == -1) return -1;

            var occurrence = 1;

            for (var i = 0; i < index; i++)
            {
                if (All[i].Name == e.Name)
                {
                    occurrence++;
                }
            }

            return occurrence;
        }


        internal static bool AreNullable(string left, XName right)
        {
            if (string.IsNullOrWhiteSpace(left) || right == null) return false;

            if (left.ToLower().Contains("x:null"))
            {
                if (right.LocalName.ToLower() == "null" && (right.NamespaceName.ToLower() == Xaml2006Uri || right.NamespaceName.ToLower() == Xaml2009Uri))
                {
                    return true;
                }    
            }

            return false;
        }


        internal static bool IsXamlSchema(XName name)
        {
            if (string.IsNullOrWhiteSpace(name?.NamespaceName)) return false;
            return name.NamespaceName == Xaml2006Uri || name.NamespaceName == Xaml2009Uri;
        }
    }
}
