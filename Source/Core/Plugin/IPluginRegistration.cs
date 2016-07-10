using XenForms.Core.Toolbox;

namespace XenForms.Core.Plugin
{
    public interface IPluginRegistration
    {
        string AutoUpdateUrl { get; }
        string PluginName { get; }
        string Version { get; }
        string Author { get; }
        string WebSite { get; }
        string UniqueId { get; }

        ViewRegistration[] Views { get; }
        void Register(PropertyEditorManager manager);
    }
}