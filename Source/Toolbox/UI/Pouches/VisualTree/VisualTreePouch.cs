using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using Ninject;
using XenForms.Core.Plugin;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Toolbox.Shell;
using XenForms.Toolbox.UI.Shell;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Pouches.VisualTree
{
    public class VisualTreePouch : XenVisualElement
    {
        private TableLayout _container;
        private TreeGridView _tree;
        private readonly ContextMenu _contextMenu;

        private int _views;
        private int _lastSelected = -1;

        private readonly IEnumerable<MenuItem> _editMenuItems;
        private readonly RegisterNewViewDialog _newViewDlg;


        public VisualTreePouch(IEnumerable<IPluginRegistration> plugins, MenuBuilder menuBuilder)
        {
            _newViewDlg = ToolboxApp.Services.Get<RegisterNewViewDialog>();

            var views = plugins
                .SelectMany(p => p.Views)
                .OrderBy(o => o.Type.ShortName);

            var menuItems = menuBuilder.Build(MenuLocation.VisualTree, views);
            _editMenuItems = menuBuilder.Build(MenuLocation.VisualTree, views);

            var mis = menuItems?.ToArray();

            if (mis != null)
            {
                _newViewDlg.Register(mis);
                _newViewDlg.Register(_editMenuItems);
                _contextMenu = new ContextMenu(mis);
            }

            // Once the toolbox sends the visual tree, we'll show it.
            ToolboxApp.Bus.Listen<NewVisualTreeRootSet>(args =>
            {
                _views = 0;
                Application.Instance.Invoke(PopulateVisualTree);
            });

            ToolboxApp.Bus.Listen<ShowRegisterNewViewDialog>(args =>
            {
                _newViewDlg.Show();
            });

            ToolboxApp.Bus.Listen<ProjectFolderSetEvent>(args =>
            {
                _newViewDlg.Initialize();
            });
        }


        protected override void OnUnload()
        {
            base.OnUnload();
            ToolboxApp.Bus.StopListening<NewVisualTreeRootSet>();
            ToolboxApp.Bus.StopListening<ShowRegisterNewViewDialog>();
            ToolboxApp.Bus.StopListening<ProjectFolderSetEvent>();
        }


        protected override Control OnDefineLayout()
        {
            _container = new TableLayout(1, 2);

            var header = new Label
            {
                Text = "Visual Tree"
            };

            var actionBar = new TableLayout(5, 1)
            {
                Spacing = new Size(5, 0),
                Padding = new Padding(0, 0, 0, 5)
            };

            var refreshBtn = new ImageButton
            {
                ToolTip = "Refresh",
                Image = AppImages.Refresh,
                Width = 18,
                Height = 18
            };

            var addBtn = new ImageButton
            {
                ToolTip = "Register New View",
                Image = AppImages.AddView,
                Width = 18,
                Height = 18
            };

            var editBtn = new ButtonMenuList
            {
                ToolTip = "Edit View",
                Image = AppImages.Edit,
                Width = 18,
                Height = 18
            };

            if (_editMenuItems != null)
            {
                editBtn.AddRange(_editMenuItems);
            }

            // visual tree
            _tree = new TreeGridView
            {
                ShowHeader = false,
                ContextMenu = _contextMenu
            };

            _tree.Columns.Add(new GridColumn { DataCell = new VisualTreeCell(), HeaderText = "Image and Text", AutoSize = true, Resizable = true, Editable = false });

            _tree.SelectionChanged += (s, e) =>
            {
                // An item has been selected
                var ti = _tree.SelectedItem as TreeGridItem;

                if (ti?.Tag != null)
                {
                    // does this visual tree node have the data we need?
                    var node = (VisualTreeNode) ti.Tag;

                    if (_tree.SelectedRows.Any())
                    {
                        _lastSelected = _tree.SelectedRows.First();

                        // notify outside views
                        var evt = new VisualTreeNodeSelected(node);
                        ToolboxApp.Bus.Notify(evt);
                    }
                }
                else
                {
                    ToolboxApp.Log.Error("The selected node did not have an attached widget.");
                }
            };

            // layout
            actionBar.Add(header, 0, 0, true, false);
            actionBar.Add(addBtn, 1, 0, false, false);
            actionBar.Add(editBtn, 2, 0, false, false);
            actionBar.Add(refreshBtn, 3, 0, false, false);
            actionBar.Add(null, 4, 0, false, false);

            _container.Add(actionBar, 0, 0, false, false);
            _container.Add(_tree, 0, 1, false, true);

            addBtn.Click += OnAddViewClicked;

            refreshBtn.Click += (s, e) =>
            {
                ToolboxApp.Bus.Notify(new ForceVisualTreeRefresh());
                ToolboxApp.Bus.Notify(new ShowStatusMessage("Visual Tree refresh requested"));
            };

            return _container;
        }


        private void OnAddViewClicked(object sender, EventArgs e)
        {
            _newViewDlg.Show();
        }


        /// <summary>
        /// For all the widgets received from the designer, create a treeitem for each and populate
        /// the tree view.
        /// </summary>
        private void PopulateVisualTree()
        {
            var root = Core.Toolbox.VisualTree.Root;

            // The first visible node in the grid
            var item = new TreeGridItem
            {
                Tag = root,
                Expanded = true
            };

            // this node is not visible, but all other nodes are children of it.
            var rootTreeItem = new TreeGridItem();

            rootTreeItem.Children.Add(item);
            AddNodeToVisualTree(root, item);

            _tree.DataStore = rootTreeItem;

            // select the first tree item, if exists.
            if (_tree.DataStore.Count > 0 && rootTreeItem.Count > 0)
            {
                if (_lastSelected == -1)
                {
                    // when first connected
                    _tree.SelectedItem = rootTreeItem.Children[0];
                }
                else
                {
                    int select;

                    if (_lastSelected > _views)
                    {
                        select = _lastSelected - 1;
                    }
                    else
                    {
                        select = _lastSelected;
                    }

                    _tree.SelectRow(select);
                }
            }
        }


        /// <summary>
        /// 1. Add a visual tree node to a tree item.
        /// 2. Find the imediate children and add them as a child.
        /// 3. Repeat.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="parentItem"></param>
        private void AddNodeToVisualTree(VisualTreeNode parentNode, TreeGridItem parentItem)
        {
            // If the parentNode doesn't have any children, no need to continue.
            if (parentNode.Widget?.Children == null || !parentNode.Widget.Children.Any())
            {
                return;
            }

            foreach (var child in parentNode.Widget.Children)
            {
                _views++;

                var childNode = new VisualTreeNode
                {
                    Widget = child
                };

                var childGridItem = new TreeGridItem
                {
                    Tag = childNode,
                    Expanded = true
                };

                parentItem.Children.Add(childGridItem);
                AddNodeToVisualTree(childNode, childGridItem);
            }
        }
    }
}