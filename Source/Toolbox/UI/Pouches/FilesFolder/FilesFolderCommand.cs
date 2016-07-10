using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Networking;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Toolbox.Project;
using XenForms.Core.Toolbox.Shell;

namespace XenForms.Toolbox.UI.Pouches.FilesFolder
{
    public abstract class FilesFolderCommand : Command
    {
        protected ProjectWorkspace Project { get; }
        protected readonly IMessageBus Messaging;
        protected readonly ToolboxSocket Socket;

        protected string SelectedFolder { get; set; }
        protected string SelectedFile { get; set; }
        protected Image Icon { get; set; }


        protected FilesFolderCommand(ProjectWorkspace project, IMessageBus messaging, ToolboxSocket socket)
        {
            Project = project;
            Messaging = messaging;
            Socket = socket;

            messaging.Listen<ProjectFileSelected>(payload =>
            {
                SelectedFile = payload.File.FullPath;
                SelectedFolder = string.Empty;

                SetProjectData();
            });

            messaging.Listen<ProjectFolderSelected>(payload =>
            {
                SelectedFolder = payload.Folder.FullPath;
                SelectedFile = string.Empty;

                SetProjectData();
            });
        }


        private void SetProjectData()
        {
            Project.State.SelectedFile = SelectedFile;
            Project.State.SelectedFolder = SelectedFolder;
        }
    }
}