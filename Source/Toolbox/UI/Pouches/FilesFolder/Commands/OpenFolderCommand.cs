using System;
using System.Diagnostics;
using Eto.Forms;
using XenForms.Core.Networking;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Toolbox.Project;
using XenForms.Core.Toolbox.Shell;

namespace XenForms.Toolbox.UI.Pouches.FilesFolder.Commands
{
    [MenuPlacement("faa45389-c95a-4368-8b46-0cd1b2d358e6", MenuLocation.FilesFolders, "Explore")]
    public class OpenFolderCommand : FilesFolderCommand
    {
        public OpenFolderCommand(ProjectWorkspace project, IMessageBus messaging, ToolboxSocket socket) 
            : base(project, messaging, socket)
        {
            MenuText = "Open in File Explorer";
        }


        public override MenuItem CreateMenuItem()
        {
            var menu = base.CreateMenuItem();
            
            menu.Validate += (s, e) =>
            {
                menu.Enabled = !string.IsNullOrWhiteSpace(SelectedFolder) && Socket.IsConnected;
            };

            return menu;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);
            Process.Start(SelectedFolder);

            Messaging.Notify(new ShowStatusMessage($"Opened in Explorer: {SelectedFolder}"));
        }
    }
}
