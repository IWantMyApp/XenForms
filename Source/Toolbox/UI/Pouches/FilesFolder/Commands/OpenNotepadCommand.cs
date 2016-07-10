using System;
using System.Diagnostics;
using Eto.Forms;
using XenForms.Core.Networking;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Toolbox.Project;
using XenForms.Core.Toolbox.Shell;

namespace XenForms.Toolbox.UI.Pouches.FilesFolder.Commands
{
    [MenuPlacement("3e759e0f-92ab-4ce8-ad0f-7599a53971ef", MenuLocation.FilesFolders, "Explore")]
    public class OpenNotepadCommand : FilesFolderCommand
    {
        public OpenNotepadCommand(ProjectWorkspace project, IMessageBus messaging, ToolboxSocket socket)
            : base(project, messaging, socket)
        {
            MenuText = "Open in Notepad";
        }


        public override MenuItem CreateMenuItem()
        {
            var menu = base.CreateMenuItem();

            menu.Validate += (s, e) =>
            {
                menu.Enabled = Project.Supports.IsEditable(SelectedFile) && Socket.IsConnected;
            };

            return menu;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            var message = $"Opening in Notepad: {SelectedFile}";

            try
            {
                Process.Start("notepad.exe", SelectedFile);
            }
            catch (Exception ex)
            {
                message = $"Unable to open {SelectedFile} in Notepad. See the log viewer for more information.";
                ToolboxApp.Log.Error(ex, $"Error opening {SelectedFile}.");       
            }

            Messaging.Notify(new ShowStatusMessage(message));
        }
    }
}
