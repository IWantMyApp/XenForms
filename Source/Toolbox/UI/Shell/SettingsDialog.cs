using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using Ninject;
using XenForms.Core.Reflection;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Resources;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell
{
    public sealed class SettingsDialog : XenVisualElement
    {
        private const string GroupTag = "Group";
        private const string RootItemText = "Root";

        private readonly Dictionary<Type, XenVisualElement> _panelCache;
        private readonly IFindCustomAttributes<SettingsPanelAttribute> _settingAttributes;

        private Dialog _dialog;
        private TreeView _treeView;
        private Panel _settingsPanelContainer;
        private Button _cancelButton;


        public SettingsDialog(IFindCustomAttributes<SettingsPanelAttribute> settingAttributes)
        {
            if (settingAttributes == null) {throw new ArgumentNullException(nameof(settingAttributes));}

            _settingAttributes = settingAttributes;
            _panelCache = new Dictionary<Type, XenVisualElement>();
        }


        public async Task ShowAsync(Control parent)
        {
            await _dialog.ShowModalAsync(parent);
        }


        protected override Control OnDefineLayout()
        {
            var footer = CreateFooter();
            _settingsPanelContainer = new Panel {Padding = new Padding(0,0,5,0)};
            var layout = new TableLayout(1, 2);


            _dialog = new Dialog
            {
                Title = SettingsResource.Dialog_title,
                ClientSize = new Size(685, 400),
                AbortButton = _cancelButton,
                Icon = AppImages.Xf,
                ShowInTaskbar = false
            };


            _treeView = CreateTreeView();
            PopulateTreeView();


            var splitter = new Splitter
            {
                FixedPanel = SplitterFixedPanel.Panel1,
                Orientation = Orientation.Horizontal,
                Panel1 = _treeView,
                Panel2 = _settingsPanelContainer,
                Position = 200,
            };

            layout.Add(splitter, 0, 0, true, true);
            layout.Add(footer, 0, 1, false, false);

            layout.Padding = new Padding(5,5,0,0);
            _treeView.SelectionChanged += async (s, e) => await OnTreeViewSelectionChanged(s, e);
            _treeView.LoadComplete += OnTreeViewLoadComplete;
            _dialog.Content = layout;

            return _dialog;
        }


        private void CloseDialog()
        {
            _dialog.Close();
        } 


        private Control CreateFooter()
        {
            var ok = new Button { Text = CommonResource.Ok };
            _cancelButton = new Button { Text = CommonResource.Cancel };


            ok.Click += async (s, e) =>
            {
                await NotifyFormsAboutClosingTime();
                CloseDialog();
            };

            _cancelButton.Click += (s, e) => { CloseDialog(); };


            var layout = new TableLayout(3, 1)
            {
                Padding = new Padding(10),
                Spacing = new Size(5, 0)
            };

            layout.Add(null, 0 ,0, true, true);
            layout.Add(ok, 1,0, false,false);
            layout.Add(_cancelButton, 2, 0, false, false);

            return layout;
        }


        private TreeView CreateTreeView()
        {
            var root = new TreeItem
            {
                Expanded = true,
                Text = RootItemText
            };

            var tree = new TreeView
            {
                DataStore = root
            };

            return tree;
        }


        private async Task OnTreeViewSelectionChanged(object sender, EventArgs e)
        {
            var tv = sender as TreeView;

            var selected = tv?.SelectedItem as TreeItem;
            if (selected == null) return;

            Type panelType;

            /*
             *  The top level tree items "settings groups." This can be determined by inspecting the Tag property.
             *  If it contains the text "Group," it's a group. Otherwise, it should contain the type of the panel to instantiate.
             */

            if (selected.Tag.Equals(GroupTag))
            {
                var child = selected.Children[0] as TreeItem;
                if (child == null)
                {
                    ToolboxApp.Log.Warn(SettingsResource.A_child_was_not_found_for_the_settings_group, selected.Text);
                    return;
                }

                // When the group tree item is selected, we will show the settings panel for the fist child.
                panelType = child.Tag as Type;
            }
            else
            {
                panelType = selected.Tag as Type;
            }


            if (panelType == null) return;
            Control view;

            // Prevent a panel object from being created again and losing it's unsaved contents.
            if (_panelCache.ContainsKey(panelType))
            {
                view = _panelCache[panelType].View;
            }
            else
            {
                // Each settings panel can have it's own dependencies injected
                var panel = ToolboxUI.Create(panelType) as XenForm;

                if (panel == null)
                {
                    throw new ActivationException(String.Format(SettingsResource.The_panel_couldnt_be_activated,
                        selected.Text, typeof(XenVisualElement).FullName));
                }

                // Build the panel and cache it for future retrievel.
                ToolboxUI.Activate(panel);
                _panelCache[panelType] = panel;
                await panel.ShowFormAsync();

                view = panel.View;
            }

            _settingsPanelContainer.Content = view;
        }

        
        private void OnTreeViewLoadComplete(object sender, EventArgs e)
        {
            var root = _treeView.DataStore[0] as TreeItem;
            if (root == null) throw new ApplicationException(SettingsResource.Could_not_find_root_tree_item);

            var firstChild = root.Children.FirstOrDefault();

            if (firstChild != null)
            {
                _treeView.SelectedItem = firstChild;
                _treeView.RefreshData();
            }
        }


        private void PopulateTreeView()
        {
            var associations = _settingAttributes.FindAll();
            var root = _treeView.DataStore as TreeItem;

            if (associations == null || associations.Length == 0)
            {
                ToolboxApp.Log.Error(SettingsResource.The_settings_panels_were_not_found);
                throw new ApplicationException(SettingsResource.The_settings_panels_were_not_found);
            }

            if (root == null)
            {
                ToolboxApp.Log.Error(SettingsResource.The_root_item_was_not_assigned_to_the_datastore);
                throw new ApplicationException(SettingsResource.The_root_item_was_not_assigned_to_the_datastore);
            }

            // The top level tree items are known as setting groups. They are arranged in ascending ABC order.
            var groupNames = associations
                .SelectMany(a => a.Attributes)
                .Select(b => b.GroupName)
                .Distinct()
                .OrderBy(c => c);

            foreach (var groupName in groupNames)
            {
                // Create the group tree item
                var parent = new TreeItem
                {
                    Expanded = true,
                    Text = groupName,
                    Tag = GroupTag
                };

                // The root tree item is irrelevant for processing, but its required for the ui control
                root.Children.Add(parent);

                // Based on the current groupName, order the children panels in ascending ABC order
                var orderedAssociations = associations
                    .Where(a => a.Attributes.Select(s => s.GroupName).Contains(groupName))
                    .Select(a => new
                    {
                        a.DecoratedType,
                        Attributes = a.Attributes.OrderBy(c => c.PanelName)
                    })
                    .ToArray();

                foreach (var association in orderedAssociations)
                {
                    foreach (var attribute in association.Attributes)
                    {
                        var child = new TreeItem
                        {
                            Text = attribute.PanelName,
                            
                            // This is the control that will be instantiated & cached for retrievel.
                            // It will be shown to the user when the tree item is selected.
                            Tag = association.DecoratedType
                        };

                        parent.Children.Add(child);
                    }
                }
            }
        }


        /// <summary>
        /// Notify the setting panels that they can save their state to the database
        /// </summary>
        public async Task NotifyFormsAboutClosingTime()
        {
            foreach (var panel in _panelCache)
            {
                var canSave = panel.Value as XenForm;
                if (canSave == null) continue;
                await canSave.CloseFormAsync(true);
            }
        }
    }
}
