using System.Text;
using System.Xml;
using Portable.Xaml;

namespace XenForms.Core.XAML
{
    public class XenXamlWriter
    {
        public string Save(object instance)
        {
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings();
            var xw = XmlWriter.Create(sb, settings);

            var schema = new XamlSchemaContext(new XamlSchemaContextSettings());
            var xxw = new XamlXmlWriter(xw, schema);
            XamlServices.Save(xxw, instance);

            return sb.ToString();
        }
    }
}