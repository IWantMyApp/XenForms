using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.FileSystem;
using XenForms.Core.Platform.FileSystem;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Toolbox.Project;
using XenForms.Core.Toolbox.Shell;
using XenForms.Toolbox.UI.Pouches.FilesFolder.Commands;
using XenForms.Toolbox.UI.Shell;
using XenForms.Toolbox.UI.Shell.Images;
using XenForms.Toolbox.UI.Shell.Styling;


namespace XenForms.Toolbox.UI.Pouches.FilesFolder
{
    public class FilesFolderPouch : XenVisualElement
    {
        private readonly IFileSystemTraverser _fileSystemTraverser;
        private readonly LoadProjectDialog _loadProject;
        private readonly ContextMenu _contextMenu;
        private readonly Panel _visibleLayout;
        private readonly Control _minLayout;

        private TreeView _tree;
        private Control _maxLayout;
        private string _currentFolder;

        // key for blank item
        private const string Blank = "blank";


        public FilesFolderPouch(MenuBuilder menuBuilder, IFileSystemTraverser fileSystemTraverser, LoadProjectDialog loadProject)
        {
            _fileSystemTraverser = fileSystemTraverser;
            _loadProject = loadProject;

            var menuItems = menuBuilder.Build(MenuLocation.FilesFolders);

            // can be null when no plugins are loaded.
            if (menuItems != null)
            {
                _contextMenu = new ContextMenu(menuItems);
            }

            // layout that's displayed when the actionbar's "Minimize" button is clicked.
            _minLayout = CreateMinimizeLayout();

            // reference to the layout that is always visible
            _visibleLayout = new Panel();
        }


        // Set the maximized layout to current visible
        public void ResetDefaultLayout()
        {
            _visibleLayout.Content = _maxLayout;
        }


        protected override Control OnDefineLayout()
        {
            var dir = ToolboxApp.Settings.GetString(UserSettingKeys.Builtin.DefaultProjectDirectory);

            // default directory not set in the settings dialog
            if (string.IsNullOrWhiteSpace(dir))
            {
                ShowEmptyPanel("Open a folder that contains your existing XAML. Selecting your solution folder is likely the best choice.");
            }
            else
            {
                ShowProjectFolder(dir);
            }

            return _visibleLayout;
        }


        protected override void OnUnload()
        {
            base.OnUnload();

            ToolboxApp.Bus.StopListening<ProjectFileSelected>();
            ToolboxApp.Bus.StopListening<ProjectFolderSelected>();
        }


        /// <summary>
        /// When a folder or file is selected, emit messages that can be listened for by other
        /// interested parties in the application.
        /// </summary>
        private void OnTreeItemClicked()
        {
            var item = _tree.SelectedItem as TreeItem;
            var file = item?.Tag as FileDesc;
            var folder = item?.Tag as FolderDesc;

            if (file != null)
            {
                ToolboxApp.Bus.Notify(new ProjectFileSelected(file));
            }

            if (folder != null)
            {
                ToolboxApp.Bus.Notify(new ProjectFolderSelected(folder));
            }

            if (file == null && folder == null)
            {
                ToolboxApp.Log.Warn($"Failed notifying that project item selected: {_tree.SelectedItem.Text}");
            }
        }


        /// <summary>
        /// This is the event handler for the "Open Folder" button on the actionbar.
        /// </summary>
        private void OnOpenFolderClicked()
        {
            var dialog = new SelectFolderDialog();
            var result = dialog.ShowDialog(Application.Instance.MainForm);

            if (result == DialogResult.Ok)
            {
                ShowProjectFolder(dialog.Directory);
            }
        }


        /// <summary>
        /// To save processing time, subfolders are traversed when the user choses to expand them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFolderExpanding(object sender, TreeViewItemCancelEventArgs e)
        {
            var item = e.Item as TreeItem;
            var folder = item?.Tag as FolderDesc;
            if (folder == null) return;

            AttachFolders(item, folder.FullPath);

            if (folder.FullPath != _currentFolder)
            {
                item.Image = AppImages.OpenFolder;
            }

            RemoveBlank(item);
        }


        private void OnFolderCollapsing(object sender, TreeViewItemCancelEventArgs e)
        {
            var item = e.Item as TreeItem;
            var folder = item?.Tag as FolderDesc;
            if (folder == null) return;

            if (folder.FullPath != _currentFolder)
            {
                item.Image = AppImages.ClosedFolder;
            }

            _tree.RefreshItem(item);
        }


        /// <summary>
        /// This view is shown when a folder hasn't been selected for viewing.
        /// The user cannot minimize this view.
        /// </summary>
        /// <param name="message"></param>
        private void ShowEmptyPanel(string message)
        {
            var stack = new StackLayout
            {
                Padding = new Padding(10),
                Spacing = 10,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            var lbl = new Label { Text = message };
            var btn = new Button { Text = "Open Folder" };

            btn.Click += (sender, args) => OnOpenFolderClicked();

            stack.Items.Add(lbl);
            stack.Items.Add(btn);

            SetMaxLayout(stack);
            
            ToolboxApp.Bus.Notify(new ShowStatusMessage(ShowStatusMessage.Ready));
        }


        /// <summary>
        /// Set the visible layout.
        /// This method is important because the max layout must also be set.
        /// </summary>
        /// <param name="control"></param>
        private void SetMaxLayout(Control control)
        {
            _maxLayout = control;
            _visibleLayout.Content = _maxLayout;
        }


        /// <summary>
        /// Show the folders and files.
        /// </summary>
        /// <param name="path">Directory fullpath</param>
        private void ShowProjectFolder(string path)
        {
            _currentFolder = path;
            ToolboxApp.Project.State.ProjectFolder = _currentFolder;

            if (string.IsNullOrWhiteSpace(path)) return;

            var container = new TableLayout(1, 2);
            var actionBar = new TableLayout(7, 1)
            {
                Spacing = new Size(5,0),
                Padding = new Padding(0, 0, 0, 5)
            };

            var supporting = new Label
            {
                Text = "Project Files"
            };

            var loadProjectBtn = new ImageButton
            {
                ToolTip = "Load Project Assemblies",
                Image = AppImages.Upload,
                Width = AppStyles.IconWidth,
                Height = AppStyles.IconHeight
            };

            var openBtn = new ImageButton
            {
                ToolTip = "Open Folder",
                Image = AppImages.OpenFolder,
                Width = AppStyles.IconWidth,
                Height = AppStyles.IconHeight,
            };

            var refreshBtn = new ImageButton
            {
                ToolTip = "Refresh Folders",
                Image = AppImages.RefreshFolders,
                Width = AppStyles.IconWidth,
                Height = AppStyles.IconHeight
            };

            var minimizeBtn = new ImageButton
            {
                ToolTip = "Minimize",
                Image = AppImages.Compress,
                Width = AppStyles.IconWidth,
                Height = AppStyles.IconHeight
            };

            var setHomeBtn = new ImageButton
            {
                ToolTip = "Set as Default Folder",
                Image = AppImages.Home,
                Width = AppStyles.IconWidth,
                Height = AppStyles.IconHeight
            };

            _tree = new TreeView
            {
                ContextMenu = _contextMenu
            };

            _tree.MouseDoubleClick += OnItemDoubleClicked;

            refreshBtn.Click += (s, e) => SetTreeDataStore(_currentFolder);
            setHomeBtn.Click += (s,e) => HomeButtonCommand.Execute(_currentFolder);
            loadProjectBtn.Click += (s,e) => OnLoadProject();
            openBtn.Click += (s, e) => OnOpenFolderClicked();
            minimizeBtn.Click += (s, e) => OnToggleProjectFiles();
            _tree.SelectionChanged += (s, e) => OnTreeItemClicked();

            _tree.Expanding += OnFolderExpanding;
            _tree.Collapsing += OnFolderCollapsing;

            actionBar.Add(supporting, 0, 0, true, false);
            actionBar.Add(setHomeBtn, 1, 0, false, false);
            actionBar.Add(loadProjectBtn, 2, 0, false, false);
            actionBar.Add(refreshBtn, 3, 0, false, false);
            actionBar.Add(openBtn, 4, 0, false, false);
            actionBar.Add(minimizeBtn, 5, 0, false, false);
            actionBar.Add(null, 6, 0, false, false);

            container.Add(actionBar, 0, 0, true, false);
            container.Add(_tree, 0, 1, true, true);

            if (SetTreeDataStore(path))
            {
                SetMaxLayout(container);
            }
        }


        /// <summary>
        /// Using the path:
        ///     1. Recursively select all supported folders and files
        ///     2. Create related TreeItems
        ///     3. Set the tree's data store property
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True, if the tree contains folders or files; false, otherwise.</returns>
        private bool SetTreeDataStore(string path)
        {
            var rootItem = new TreeItem();
            AttachFolders(rootItem, path, true);

            // no children?
            if (rootItem.Count == 0)
            {
                var message = "The selected folder does not contain any supported files.";

                if (!Directory.Exists(path))
                {
                    message = "The saved folder location does not exist. Change this path in the settings dialog.";
                }

                _tree.DataStore = null;
                ShowEmptyPanel(message);

                return false;
            }

            // children exist.
            // expand first folder, if possible.
            if (rootItem.Count > 0)
            {
                var folderItem = rootItem.Children[0] as TreeItem;
                var folderData = folderItem?.Tag as FolderDesc;
                if (folderData == null) return false;

                folderItem.Expanded = true;
                folderItem.Image = AppImages.ProjectFolder;
            }

            _tree.DataStore = rootItem;

            ToolboxApp.Log.Trace($"Traversing project folder: {path}.");
            ToolboxApp.Bus.Notify(new ShowStatusMessage($"Project folder: {path}"));

            return true;
        }


        /// <summary>
        /// This layout is created once during the pouches' lifetime.
        /// It is shown when the actionbar's "Minimize" button has been clicked.
        /// </summary>
        /// <returns></returns>
        private Control CreateMinimizeLayout()
        {
            var stack = new StackLayout
            {
                Padding = new Padding(0, 25, MainForm.SplitterWidth, 0),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Top
            };

            var restoreBtn = new ImageButton
            {
                ToolTip = "Restore",
                Image = AppImages.Restore,
                Width = 18,
                Height = 18
            };

            restoreBtn.Click += (s, e) =>
            {
                OnToggleProjectFiles();
            };

            stack.Items.Add(restoreBtn);

            return stack;
        }


        /// <summary>
        /// Swap the minimize and the project files layouts.
        /// </summary>
        private void OnToggleProjectFiles()
        {
            _visibleLayout.Content = _visibleLayout.Content == _maxLayout 
                ? _minLayout 
                : _maxLayout;

            ToolboxApp.Bus.Notify(new ToggleProjectFiles());
        }


        /// <summary>
        /// Add folders and files to the <paramref name="parent"/> tree item.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="path"></param>
        /// <param name="includeTopFolder"></param>
        private void AttachFolders(TreeItem parent, string path, bool includeTopFolder = false)
        {
            var current = parent;

            _fileSystemTraverser.RecurseFolder(path, includeTopFolder,
                top =>
                {
                    current = CreateTreeItems(top, parent);

                },
                sub =>
                {
                    CreateTreeItems(sub, current);
                });

            _tree.RefreshItem(parent);
            RemoveBlank(current);
        }


        /// <summary>
        /// Create tree items for the given <paramref name="folder"/>.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private TreeItem CreateTreeItems(FolderDesc folder, TreeItem parent)
        {
            if (folder == null) return null;

            var exists = parent.Children.FirstOrDefault(c => c.Text == folder.Name);

            if (exists != null)
            {
                parent.Children.Remove(exists);
            }

            var folderItem = new TreeItem
            {
                Expanded = false,
                Text = folder.Name,
                Image = AppImages.ClosedFolder,
                Tag = folder
            };

            parent.Children.Add(folderItem);

            var files = _fileSystemTraverser.EnumerateTopFiles(folder.FullPath, SupportedFileTypes.SupportedExtensions);

            foreach (var file in files)
            {
                if (string.IsNullOrWhiteSpace(file?.Name)) continue;

                var img = file.Name.EndsWith(SupportedFileTypes.AssemblyExtension) 
                    ? AppImages.Dll : AppImages.File;

                var fileItem = new TreeItem
                {
                    Text = file.Name,
                    Image = img,
                    Tag = file
                };

                folderItem.Children.Add(fileItem);
            }

            // no more travering, but if the following file types exist we need to append a fake element to the tree item
            // so that it can be expanded.
            var hasEntries = folderItem.Count == 0
                             && (folder.HasFolders || _fileSystemTraverser.FilesExist(folder.FullPath, SupportedFileTypes.SupportedExtensions));

            if (hasEntries)
            {
                var fake = new TreeItem
                {
                    Text = Blank
                };

                folderItem.Children.Add(fake);
            }

            return folderItem;
        }


        /// <summary>
        /// Since we're lazy loading folders and files, we need to create a "dummy" tree item that's used to
        /// notify the user that the folder can be expanded.
        /// When the folder is expanded, we then need to remove this "dummy" tree item and actually load the folders and files
        /// for that folder.
        /// </summary>
        /// <param name="item"></param>
        private void RemoveBlank(TreeItem item)
        {
            var blank = item.Children.FirstOrDefault(i => i.Text == Blank);
            if (blank == null) return;

            item.Children.Remove(blank);
        }


        private void OnItemDoubleClicked(object sender, MouseEventArgs mouseEventArgs)
        {
            var si = _tree.SelectedItem as TreeItem;
            var fd = si?.Tag as FileDesc;
            if (string.IsNullOrWhiteSpace(fd?.FullPath)) return;

            if (ToolboxApp.Project.Supports.IsXaml(fd.FullPath))
            {
                var result = MessageBox.Show(Application.Instance.MainForm,
                    "Are you sure you want to open this XAML file?",
                    "XenForms",
                    MessageBoxButtons.YesNo,
                    MessageBoxType.Question);

                if (result == DialogResult.Yes)
                {
                    OpenXamlModel.Open(fd.FullPath);
                }
            }
        }


        private async Task OnLoadProject()
        {
            await _loadProject.ShowAsync();
        }
    }
}