using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Eto.Forms;
using XenForms.Core.Diagnostics;
using XenForms.Core.Plugin;
using XenForms.Core.Reflection;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.Shell;
using XenForms.Toolbox.UI.Logging;
using XenForms.Toolbox.UI.Pouches.VisualTree;

namespace XenForms.Toolbox.UI.Shell
{
    public class MenuBuilder
    {
        [DebuggerDisplay("{Value.ToString()}")]
        private class BuildItem
        {
            public object Value { get; set; }
            public string[] Path { get; set; }
        }


        private readonly TypeAttributeAssociation<MenuPlacementAttribute>[] _attributes;


        public MenuBuilder(IFindCustomAttributes<MenuPlacementAttribute> finder)
        {
            _attributes = finder.FindAll();
            _attributes.LogDiagnostics(XenLogLevel.Trace);
        }


        public IEnumerable<MenuItem> Build(MenuLocation location, IEnumerable<ViewRegistration> registrations)
        {
            var views = registrations as ViewRegistration[] ?? registrations.ToArray();

            var items = views.Select(v => new BuildItem
            {
                Value = v,
                Path = v.Path
            });

            ToolboxApp.Log.Trace($"Creating plugin menu items for {location} with {views.Length} {nameof(ViewRegistration)}s.");
            var result = CreateMenuItems(a => a.Location.HasFlag(location), items);
            ToolboxApp.Log.Trace($"Done creating plugin menu items for {location}.");

            return result;
        }


        public IEnumerable<MenuItem> Build(string pluginUniqueId)
        {
            ToolboxApp.Log.Trace($"Creating plugin menu items for {pluginUniqueId}.");
            var result = CreateMenuItems(a => a.Location.HasFlag(MenuLocation.Plugin) && a.PluginUniqueId == pluginUniqueId);
            ToolboxApp.Log.Trace($"Done plugin creating menu items for {pluginUniqueId}.");

            return result;
        }


        public IEnumerable<MenuItem> Build(MenuLocation location)
        {
            ToolboxApp.Log.Trace($"Creating context menu items for {location}.");
            var found = CreateMenuItems(a => a.Location.HasFlag(location));
            ToolboxApp.Log.Trace($"Done creating menu items for {location}.");

            return found;
        }


        private IEnumerable<MenuItem> CreateMenuItems(Func<MenuPlacementAttribute, bool> filter, IEnumerable<BuildItem> items = null)
        {
            var input = _attributes
                .SelectMany(m => m.Attributes
                    .Where(filter)
                    .Select(s => new BuildItem
                    {
                        Value = m.DecoratedType,
                        Path = s.Path
                    }))
                .ToArray();

            var combined = input.Union(items ?? new BuildItem[] {});
            return CreateMenuItems(combined);
        }


        private IEnumerable<MenuItem> CreateMenuItems(IEnumerable<BuildItem> items)
        {
            // Only ButtonMenuItems are created with the attribute method
            var result = new SortedSet<ButtonMenuItem>(new ButtonMenuItemComparer());

            foreach (var i in items)
            {
                ButtonMenuItem previous = null;

                // there is only one iteration for this item.
                // it wants to be a root element, as it has no path.
                if (i.Path == null || i.Path.Length == 0)
                {
                    var type = i.Value as Type;

                    if (type != null)
                    {
                        var cmd = ToolboxApp.Services.GetService(type) as Command;
                        if (cmd != null)
                        {
                            var btn = new ButtonMenuItem(cmd);
                            result.Add(btn);
                        }
                    }

                    // no items to process; move on.
                    continue;
                }

                // this item will have multiple iterations below; one for each path.
                // Example: paths = "Create", "Layout"
                // i will have been iterated for "Create" and "Layout"
                foreach (var step in i.Path)
                {
                    var isLastStep = i.Path.Last() == step;
                    ButtonMenuItem current;

                    // First time iteration for path
                    if (previous == null)
                    {
                        // Determine if this is the first iteration, ever.
                        previous = current = result.FirstOrDefault(r => r.Text == step);

                        // first iteration ever
                        if (previous == null)
                        {
                            current = new ButtonMenuItem { Text = step };
                            result.Add(current);
                        }
                    }
                    else
                    {
                        // 2nd+ iteration, check if new menu item
                        current = previous.Items.FirstOrDefault(r => r.Text == step) as ButtonMenuItem;

                        // match not found, this is a new menu item
                        if (current == null)
                        {
                            current = new ButtonMenuItem { Text = step };
                            SortedAdd(previous.Items, current);
                        }
                    }

                    // done with this path
                    if (isLastStep)
                    {
                        var submenu = (ISubmenu) current;
                        var type = i.Value as Type;
                        var view = i.Value as ViewRegistration;

                        if (type != null)
                        {
                            var cmd = ToolboxApp.Services.GetService(type) as Command;

                            if (cmd != null)
                            {
                                SortedAdd(submenu.Items, cmd);
                            }
                        }

                        if (view != null)
                        {
                            var cmd = new CreateWidgetCommand(ToolboxApp.Bus, ToolboxApp.SocketManager.Socket, ToolboxApp.Log, view);
                            SortedAdd(submenu.Items, cmd);
                        }
                    }

                    previous = current;
                }
            }

            return result;
        }


        private void SortedAdd(MenuItemCollection items, MenuItem btn)
        {
            items.Add(btn);
            var ordered = items.OrderBy(ButtonMenuItemComparer.Prepare).ToArray();

            items.Clear();
            items.AddRange(ordered);
        }


        private void SortedAdd(MenuItemCollection items, Command cmd)
        {
            items.Add(cmd);
            var ordered = items.OrderBy(ButtonMenuItemComparer.Prepare).ToArray();

            items.Clear();
            items.AddRange(ordered);
        }
    }
}
