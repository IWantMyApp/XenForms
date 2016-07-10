using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using XenForms.Core.Plugin;
using XenForms.Toolbox.UI.Resources;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class PluginsCommand : Command
    {
        private readonly IEnumerable<IPluginRegistration> _plugins;
        private readonly MenuBuilder _menuBuilder;
        private MenuItem _baseItem;


        public PluginsCommand(IEnumerable<IPluginRegistration> plugins, MenuBuilder menuBuilder)
        {
            _plugins = plugins;
            _menuBuilder = menuBuilder;

            MenuText = AppResource.Plugins_menu_label;
        }


        public override MenuItem CreateMenuItem()
        {
            _baseItem = base.CreateMenuItem();

            // if no plugins found, disable the menu item
            if (!_plugins.Any())
            {
                _baseItem.Enabled = false;
                return _baseItem;
            }

            var asBtn = _baseItem as ButtonMenuItem;

            if (asBtn != null)
            {
                foreach (var plugin in _plugins.OrderBy(p => p.PluginName))
                {
                    var newItem = new ButtonMenuItem
                    {
                        Text = plugin.PluginName
                    };

                    asBtn.Items.Add(newItem);

                    var subMenus = _menuBuilder.Build(plugin.UniqueId).ToList();
                    if (subMenus.Any())
                    {
                        newItem.Items.AddRange(subMenus);
                    }
                }
            }

            return _baseItem;
        }
    }
}
