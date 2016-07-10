namespace XenForms.Core.Toolbox.AppEvents
{
    public class ProjectFolderSetEvent : IAppEvent
    {
        public string CurrentProjectFolder { get; set; }

        public ProjectFolderSetEvent(string currentProjectFolder)
        {
            CurrentProjectFolder = currentProjectFolder;
        }
    }
}