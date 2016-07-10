using XenForms.Core.FileSystem;

namespace XenForms.Core.Toolbox.AppEvents
{
    public class ProjectFileSelected : IAppEvent
    {
        public FileDesc File { get; set; }

        public ProjectFileSelected() {}
        public ProjectFileSelected(FileDesc file)
        {
            File = file;
        }
    }
}