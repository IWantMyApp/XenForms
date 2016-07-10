using System.Xml.Linq;

namespace XenForms.Core.XAML
{
    public interface IXamlPostProcessor
    {
        bool IsDocumentLoaded { get; }
        XDocument LoadDocument(string xml, XamlElement[] originals = null);
        bool LoadElementDefaults(string xml);
        XDocument Process();
        void Reset();
    }
}