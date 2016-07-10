using System;
using System.Linq;
using XenForms.Core.FileSystem;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Toolbox.Shell;

namespace XenForms.Core.Toolbox.Project
{
    public sealed class ProjectState
    {
        private readonly IFileSystem _fs;
        private readonly IMessageBus _bus;
        private readonly ToolboxLogging _log;
        private string _projectFolder;

        public string SelectedFile { get; internal set; }
        public string SelectedFolder { get; internal set; }
        public VisualTreeNode SelectedVisualTreeNode { get; internal set; }
        public PouchTypes SelectedPouch { get; internal set; }
        public string OpenFile { get; internal set; }


        public ProjectState(IFileSystem fs, IMessageBus bus, ToolboxLogging log)
        {
            _fs = fs;
            _bus = bus;
            _log = log;
        }


        public string ProjectFolder
        {
            get { return _projectFolder; }
            set
            {
                _projectFolder = value;
                ShouldShowProjectDialog();
                ShouldNotifyProjectFolderSet();
            }
        }


        public string XenProjectFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ProjectFolder)) return null;
                return _fs.Combine(ProjectFolder, SupportedFileTypes.XenProjectFileName);
            }
        }


        public void Reset()
        {
            OpenFile = null;
            ProjectFolder = null;
            SelectedFile = null;
            SelectedFolder = null;
            SelectedVisualTreeNode = null;
            SelectedPouch = PouchTypes.None;
        }


        private void ShouldNotifyProjectFolderSet()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ProjectFolder))
                {
                    _bus.Notify(new ProjectFolderSetEvent(ProjectFolder));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Unable to send message {nameof(ProjectFolder)}.");
            }
        }


        private void ShouldShowProjectDialog()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ProjectFolder))
                {
                    var proj = new XenProjectFile(_fs);
                    var loaded = proj.Load(XenProjectFilePath);
                    if (loaded)
                    {
                        if (proj.Assemblies != null && proj.Assemblies.Any())
                        {
                            _bus.Notify(new ShowLoadProjectDialogEvent());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Unable to load project assemblies after working folder was changed.");
            }
        }
    }
}
