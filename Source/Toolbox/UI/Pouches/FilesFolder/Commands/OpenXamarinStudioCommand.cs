using System;
using System.Diagnostics;
using System.IO;
using Eto.Forms;
using XenForms.Core.Networking;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Toolbox.Project;
using XenForms.Core.Toolbox.Shell;

namespace XenForms.Toolbox.UI.Pouches.FilesFolder.Commands
{
    [MenuPlacement("5b1b301e-2f49-4ace-9427-8de6c86af0cc", MenuLocation.FilesFolders, "Explore")]
    public class OpenXamarinStudioCommand : FilesFolderCommand
    {
        private const string Exe = "XamarinStudio.exe";

        public string InstallDir => ToolboxApp.Settings.GetString(UserSettingKeys.Builtin.XamarinStudioDirectory);
        public bool InstallDirSet => !string.IsNullOrWhiteSpace(InstallDir);


        public OpenXamarinStudioCommand(ProjectWorkspace project, IMessageBus messaging, ToolboxSocket socket) 
            : base(project, messaging, socket)
        {
            MenuText = "Open in Xamarin Studio";
        }


        public override MenuItem CreateMenuItem()
        {
            var menu = base.CreateMenuItem();

            menu.Validate += (s, e) =>
            {
                menu.Enabled =
                    Project.Supports.IsEditable(SelectedFile)
                    && Socket.IsConnected
                    && InstallDirSet;
            };

            return menu;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);
            var cmd = Path.Combine(InstallDir, Exe);

            if (File.Exists(cmd))
            {
                var args = $"--nologo \"{SelectedFile}\"";
                Process.Start(cmd, args);
                Messaging.Notify(new ShowStatusMessage($"Opening in Xamarin Studio: {SelectedFile}"));
            }
        }
    }
}