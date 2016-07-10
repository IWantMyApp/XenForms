using System;

namespace XenForms.Core.Toolbox.Shell
{
    [Flags]
    public enum MenuLocation
    {
        VisualTree = 1,
        Plugin = 2,
        FilesFolders = 4
    }


    public class MenuPlacementAttribute : Attribute
    {
        public string PluginUniqueId { get; set; }
        public MenuLocation Location { get; set; }
        public string[] Path { get; set; }

        public MenuPlacementAttribute(string pluginUniqueId, MenuLocation location, params string[] path)
        {
            PluginUniqueId = pluginUniqueId;
            Location = location;
            Path = path;
        }
    }
}