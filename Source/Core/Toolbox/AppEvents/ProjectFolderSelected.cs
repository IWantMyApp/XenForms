using XenForms.Core.FileSystem;

namespace XenForms.Core.Toolbox.AppEvents
{
    public class ProjectFolderSelected : IAppEvent
    {
        public FolderDesc Folder { get; set; }

        public ProjectFolderSelected() { }
        public ProjectFolderSelected(FolderDesc folder)
        {
            Folder = folder;
        }
    }
}