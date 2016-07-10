using System;
using Eto.Forms;
using XenForms.Core.Networking;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.Project;
using XenForms.Core.Toolbox.Shell;

namespace XenForms.Toolbox.UI.Pouches.FilesFolder.Commands
{
    [MenuPlacement("f4d4700b-cf67-4d9c-8197-56f096633b9a", MenuLocation.FilesFolders, "Designer")]
    public class OpenXamlCommand : FilesFolderCommand
    {
        public OpenXamlCommand(ProjectWorkspace project, IMessageBus messaging, ToolboxSocket socket) 
            : base(project, messaging, socket)
        {
            MenuText = "Open XAML";
            ToolTip = "Open for editing.";
        }


        public override MenuItem CreateMenuItem()
        {
            var menu = base.CreateMenuItem();

            menu.Validate += (s, e) =>
            {
                menu.Enabled = Project.Supports.IsXaml(SelectedFile) && Socket.IsConnected;
            };

            return menu;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);
            OpenXamlModel.Open(SelectedFile);
        }
    }
}