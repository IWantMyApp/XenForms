using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Eto.Forms;
using Ninject;
using Squirrel;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Toolbox.UI.Pouches.AttachedProperties;
using XenForms.Toolbox.UI.Pouches.Events;
using XenForms.Toolbox.UI.Pouches.FilesFolder;
using XenForms.Toolbox.UI.Pouches.FilesFolder.Commands;
using XenForms.Toolbox.UI.Pouches.Properties;
using XenForms.Toolbox.UI.Pouches.VisualTree;
using XenForms.Toolbox.UI.Resources;
using XenForms.Toolbox.UI.Shell.MenuItems;

namespace XenForms.Toolbox.UI.Shell
{
    public partial class MainForm : Form
    {
        private VisualTreePouch _vtPouch;
        private PropertiesPouch _propertiesPouch;
        private FilesFolderPouch _filesFolderPouch;
        private AttachedPropertiesPouch _attachedPropertiesPouch;
        private EventsPouch _eventsPouch;


        public MainForm()
        {
            ToolboxApp.Log.Info("Creating workspace.");
            ToolboxApp.AppEvents.ApplicationInitialize(o => ToolboxApp.Services.Inject(o));
            
            SubscribeToWorkspaceEvents();

            Shown += async (sender, args) =>
            {
                ToolboxApp.Bus.Notify(new ExecuteStartupTasks());
                await CheckForUpdatesAsync();
            };

            Closing += (sender, args) =>
            {
                OnMainFormClosing(args);
            };

            Content = CreateDisconnectedWorkspaceView();
        }


        private async Task CheckForUpdatesAsync()
        {
            try
            {
                var uri = UpdateCommand.GetUpdateUri();
                if (string.IsNullOrWhiteSpace(uri)) return;

                UpdateInfo result;
                
                using (var mgr = new UpdateManager(uri))
                {
                    result = await mgr.CheckForUpdate();
                }

                if (result?.ReleasesToApply != null && result.ReleasesToApply.Any())
                {
                    Application.Instance.Invoke(() =>
                    {
                        var cmd = ToolboxApp.Services.Get<UpdateCommand>();
                        cmd.Execute();
                    });
                }
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, "Error checking for updates.");
            }
        }


        /// <summary>
        /// When these events occur, the workspace UI should update.
        /// </summary>
        private void SubscribeToWorkspaceEvents()
        {
            Application.Instance.Terminating += (sender, args) =>
            {
                ToolboxApp.IsTerminating = true;
            };

            ToolboxApp.Bus.Listen<InitializeWorkspace>(e =>
            {
                Application.Instance.Invoke(ExecuteConnectedTasks);
            });

            ToolboxApp.Bus.Listen<CleanUpWorkspace>(e =>
            {
                if (ToolboxApp.IsTerminating) return;
                Application.Instance.Invoke(ExecuteDisconnectedTasks);
            });

            ToolboxApp.Bus.Listen<ToggleProjectFiles>(e =>
            {
                OnToggleDock();
            });

            ToolboxApp.Bus.Listen<ResetWindowLayout>(e =>
            {
                OnResetWindowLayout();
            });

            ToolboxApp.Bus.Listen<RestartDesignerEvent>(e =>
            {
                _promptDesignerRestart = true;
            });

            ToolboxApp.Bus.Listen<ShowLoadProjectDialogEvent>(e =>
            {
                Application.Instance.AsyncInvoke(OnLoadProjectDialog);
            });
        }


        private async void OnLoadProjectDialog()
        {
            try
            {
                var dlg = ToolboxApp.Services.Get<LoadProjectDialog>();
                await dlg.ShowAsync();
            }
            catch (Exception)
            {
                // ignored
            }
        }


        /// <summary>
        /// Executed after the user was disconnected from a designer.
        /// </summary>
        private void ExecuteDisconnectedTasks()
        {
            // remove disconnect & insert connect
            ToolBar.Items.RemoveAt(0);
            ToolBar.Items.Insert(0, _connect);

            // Remove pouches
            Content = null;

            // Update the workspace to reflect that we have disconnected from a designer.
            Content = CreateDisconnectedWorkspaceView();

            var pluginItem = Menu.Items.FirstOrDefault(n => n.Text == AppResource.Plugins_menu_label);

            if (pluginItem != null)
            {
                // disable menu item & remove reference to command
                pluginItem.Enabled = false;
                pluginItem.Command = null;
            }

            _save.Enabled = false;
            _newPage.Enabled = false;

            Title = AppResource.Title_disconnected;
        }


        /// <summary>
        /// Executed after a successful connection to a designer.
        /// </summary>
        private void ExecuteConnectedTasks()
        {
            // remove disconnect & insert connect
            ToolBar.Items.RemoveAt(0);
            ToolBar.Items.Insert(0, _disconnect);

            // Remove disconnect view
            Content = null;

            // Create connected view
            Content = CreateConnectedWorkspaceView();

            // Remove old plugins menu item
            var pluginItem = Menu.Items.FirstOrDefault(n => n.Text == AppResource.Plugins_menu_label);

            if (pluginItem != null)
            {
                var index = Menu.Items.IndexOf(pluginItem);
                Menu.Items.Remove(pluginItem);

                // Insert new plugins menu item, retaining it's position in the menubar
                var cmd = ToolboxApp.Services.Get<PluginsCommand>();
                Menu.Items.Insert(index, cmd);

                // todo: enable for release 1
                cmd.Enabled = false;
            }
            else
            {
                ToolboxApp.Log.Error("The 'Plugins' menu item was not found. All design commands are not shown.");
            }

            _save.Enabled = true;
            _newPage.Enabled = true;

            ToolboxApp.Bus.Notify(new ShowStatusMessage("Ready"));
        }


        private void OnMainFormClosing(CancelEventArgs args)
        {
            var quit = DialogResult.Yes;

            // show msgbox if connected to a host
            if (ToolboxApp.SocketManager.IsConnected)
            {
                quit = QuitCommand.ShowPrompt();
            }

            // quit
            if (quit == DialogResult.Yes)
            {
                ToolboxApp.AppEvents.ApplicationCleanUp();
            }
            else
            {
                args.Cancel = true;
            }
        }
    }
}