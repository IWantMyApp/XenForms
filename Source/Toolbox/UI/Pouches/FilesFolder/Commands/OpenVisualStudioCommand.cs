using System;
using Eto.Forms;
using XenForms.Core.Networking;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Toolbox.Project;
using XenForms.Core.Toolbox.Shell;
using XenForms.Toolbox.UI.VisualStudio;

namespace XenForms.Toolbox.UI.Pouches.FilesFolder.Commands
{
    [MenuPlacement("48a6e62a-353a-48ac-ad19-7b89ddbe969e", MenuLocation.FilesFolders, "Explore")]
    public class OpenVisualStudioCommand : FilesFolderCommand
    {
        public OpenVisualStudioCommand(ProjectWorkspace project, IMessageBus messaging, ToolboxSocket socket) 
            : base(project, messaging, socket)
        {
            MenuText = "Open in Visual Studio";
        }


        public override MenuItem CreateMenuItem()
        {
            var menu = base.CreateMenuItem();

            menu.Validate += (s, e) =>
            {
                menu.Enabled = 
                    Project.Supports.IsEditable(SelectedFile)
                    && Socket.IsConnected
                    && VisualStudioBridge.IsInstalled(VisualStudioVersion.VS2015);
            };

            return menu;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            var opened = VisualStudioBridge.OpenFile(SelectedFile);
            var message = $"Opening in Visual Studio: {SelectedFile}";

            if (!opened)
            {
                message = $"Failed to load {SelectedFile} in Visual Studio.";
            }

            Messaging.Notify(new ShowStatusMessage(message));
        }
    }
}
