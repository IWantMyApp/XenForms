using System;
using Eto.Drawing;
using Eto.Forms;
using Ninject;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Pouches.AttachedProperties;
using XenForms.Toolbox.UI.Pouches.Events;
using XenForms.Toolbox.UI.Pouches.FilesFolder;
using XenForms.Toolbox.UI.Pouches.Properties;
using XenForms.Toolbox.UI.Pouches.VisualTree;
using XenForms.Toolbox.UI.Resources;
using XenForms.Toolbox.UI.Shell.Devices;
using XenForms.Toolbox.UI.Shell.Images;
using XenForms.Toolbox.UI.Shell.MenuItems;
using XenForms.Toolbox.UI.Shell.Pouches;

namespace XenForms.Toolbox.UI.Shell
{
    public partial class MainForm
    {
        public const int SplitterWidth = 5;
        public const int DockWidth = 25;
        public const int FormWidth = 1025;
        public const int FormHeight = 700;
        public const double DefaultCenterSplitterPosition = FormWidth*.47;
        public const double DefaultLeftSplitterPosition = DefaultCenterSplitterPosition*.5;

        private Splitter _centerSplit;
        private Splitter _leftSplit;
        private ConnectCommand _connect;
        private DisconnectCommand _disconnect;
        private SaveXamlCommand _save;
        private NewPageCommand _newPage;
        private bool _promptDesignerRestart;


        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);

            // Menu Commands
            _connect = ToolboxApp.Services.Get<ConnectCommand>();
            _disconnect = ToolboxApp.Services.Get<DisconnectCommand>();

            var log = ToolboxApp.Services.Get<ShowLogViewerCommand>();
            var quit = ToolboxApp.Services.Get<QuitCommand>();
            var settings = ToolboxApp.Services.Get<SettingsCommand>();
            var onTop = ToolboxApp.Services.Get<ToggleStayOnTopCommand>();
            _newPage = ToolboxApp.Services.Get<NewPageCommand>();
            _save = ToolboxApp.Services.Get<SaveXamlCommand>();

            var feedback = new FeedbackCommand();
            var update = new UpdateCommand();
            var releaseNotes = new ReleaseNotesCommand();
            var lpac = new LoadProjectAssembliesCommand();
            var newType = new RegisterNewTypeCommand();
            var newView = new RegisterNewViewCommand();

            // Styling
            _save.Enabled = false;
            _newPage.Enabled = false;

            Title = AppResource.Title_disconnected;
            Padding = new Padding(5);
            Maximizable = false;
            Resizable = true;
            Icon = AppImages.Xf;
            ClientSize = new Size(FormWidth, FormHeight);
            Location = GetFormLocation();
            
            // Menubar
            Menu = new MenuBar
            {
                Items =
                {
                    // File
                    new ButtonMenuItem
                    {
                        Text = AppResource.File_menu_label,
                        Items =
                        {
                            _connect,
                            _disconnect,
                            new SeparatorMenuItem(),
                            _newPage,
                            new SeparatorMenuItem(),
                            _save
                        }
                    },

                    new ButtonMenuItem
                    {
                        Text = "&Edit",
                        Items =
                        {
                            new ButtonMenuItem { Text = "&Undo", Enabled = false },
                            new ButtonMenuItem { Text = "&Redo", Enabled = false },
                            new SeparatorMenuItem(),
                            new ButtonMenuItem { Text = "&Cut", Enabled = false},
                            new ButtonMenuItem { Text = "&Copy", Enabled = false},
                            new ButtonMenuItem { Text = "&Paste", Enabled = false},
                        }
                    },

                    // Tools
                    new ButtonMenuItem
                    {
                        Text = AppResource.Tools_menu_label,
                        Items =
                        {
                            newView,
                            newType,
                            lpac,
                            new SeparatorMenuItem(),
                            settings
                        }
                    },

                    // Plugins
                    // This item will be replaced on connect & disconnect from a design server
                    new ButtonMenuItem
                    {
                        Text = AppResource.Plugins_menu_label,
                        Enabled = false
                    },

                    // Window
                    new ButtonMenuItem
                    {
                        Text = AppResource.Window_menu_label,
                        Items =
                        {
                            onTop,
                            new SeparatorMenuItem(),
                            new ResetViewCommand()
                        }
                    }
                },

                // Help
                HelpItems =
                {
                    feedback,
                    releaseNotes,
                    log,
                    new SeparatorMenuItem(),
                    update
                },

                QuitItem = quit,
                AboutItem = new AboutCommand(),
            };

            ToolBar = new ToolBar
            {
                Items =
                {
                    _connect,
                    _newPage,
                    _save,
                    settings,
                    new SeparatorToolItem(),
                    onTop,
                    feedback,
                }
            };

            Application.Instance.Terminating += (s, _) => ToolboxApp.IsTerminating = true;
        }


        /// <summary>
        /// Update the workspace to reflect that the toolbox has disconnected from a design surface.
        /// </summary>
        /// <returns></returns>
        private Control CreateDisconnectedWorkspaceView()
        {
            var manager = new DeviceManagerView(_promptDesignerRestart);
            manager.Activate();

            // reset value
            _promptDesignerRestart = false;
            return manager.View;
        }

        
        /// <summary>
        /// Update the workspace to reflect that the toolbox has connected to a design surface.
        /// </summary>
        /// <returns></returns>
        private Control CreateConnectedWorkspaceView()
        {
            var statusbar = new StatusBar();
            var layout = new TableLayout(1, 2);

            _centerSplit = new Splitter
            {
                Panel1 = CreateLeftPanel(),
                Panel2 = CreateCenterPanel(),
                FixedPanel = SplitterFixedPanel.Panel1,
                Orientation = Orientation.Horizontal,
                RelativePosition = DefaultCenterSplitterPosition,
            };

            ToolboxUI.Activate(statusbar);

            layout.Add(_centerSplit, 0, 0, true, true);
            layout.Add(statusbar.View, 0, 1, true, false);

            Title = AppResource.Title_connected;
            return layout;
        }


        private void OnToggleDock()
        {
            var inDefaultPos = Math.Abs(_leftSplit.RelativePosition - DefaultLeftSplitterPosition) < 0.1;

            if (inDefaultPos)
            {
                _leftSplit.RelativePosition = DockWidth;
                _leftSplit.SplitterWidth = 0;
            }
            else
            {
                _leftSplit.RelativePosition = DefaultLeftSplitterPosition;
                _leftSplit.SplitterWidth = SplitterWidth;
            }
        }


        private void OnResetWindowLayout()
        {
            _leftSplit.Position = (int) DefaultLeftSplitterPosition;
            _leftSplit.RelativePosition = DefaultLeftSplitterPosition;

            _centerSplit.Position = (int) DefaultCenterSplitterPosition;
            _centerSplit.RelativePosition = DefaultCenterSplitterPosition;

            _filesFolderPouch.ResetDefaultLayout();
        }


        private Control CreateLeftPanel()
        {
            _filesFolderPouch = ToolboxApp.Services.Get<FilesFolderPouch>();
            _vtPouch = ToolboxApp.Services.Get<VisualTreePouch>();

            ToolboxUI.Activate(_filesFolderPouch);
            ToolboxUI.Activate(_vtPouch);

            _leftSplit = new Splitter
            {
                FixedPanel = SplitterFixedPanel.Panel1,
                Orientation = Orientation.Horizontal,
                RelativePosition = DefaultLeftSplitterPosition,
                Panel1 = _filesFolderPouch.View,
                Panel2 = _vtPouch.View
            };


            return _leftSplit;
        }


        private Control CreateCenterPanel()
        {
            _attachedPropertiesPouch = ToolboxApp.Services.Get<AttachedPropertiesPouch>();
            _propertiesPouch = ToolboxApp.Services.Get<PropertiesPouch>();
            _eventsPouch = ToolboxApp.Services.Get<EventsPouch>();
            var toolbelt = ToolboxApp.Services.Get<Toolbelt>();

            toolbelt.Add(_propertiesPouch);
            toolbelt.Add(_attachedPropertiesPouch);
            toolbelt.Add(_eventsPouch);
            ToolboxUI.Activate(toolbelt);

            return toolbelt.View;
        }


        private Point GetFormLocation()
        {
            var workingArea = Screen.PrimaryScreen.WorkingArea;
            var posX = (int)(workingArea.Width - ClientSize.Width) / 2;
            var posY = (int)(workingArea.Height - ClientSize.Height) / 4;

            return new Point(posX, posY);
        }
    }
}