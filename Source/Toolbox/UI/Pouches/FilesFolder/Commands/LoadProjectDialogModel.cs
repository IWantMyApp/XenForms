using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Eto.Forms;
using XenForms.Core;
using XenForms.Core.FileSystem;
using XenForms.Core.Messages;
using XenForms.Core.Platform;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.Project;
using XenForms.Toolbox.UI.Properties;

namespace XenForms.Toolbox.UI.Pouches.FilesFolder.Commands
{
    public class LoadProjectDialogModel : INotifyPropertyChanged
    {
        private readonly IFileSystem _fs;

        private bool _ignoreSystemAssemblies;
        private bool _ignorePackageFolder;
        private bool _includeDebugAssemblies;
        private bool _includeReleaseAssemblies;
        private string _searchText;


        public IEnumerable<ProjectAssembly> IncludedAssemblies
        {
            get
            {
                return Assemblies?.Where(a => a.Include);
            }
        }


        public FilterCollection<ProjectAssembly> Assemblies { get; }
        public bool IsReady { get; set; }


        public LoadProjectDialogModel(IFileSystem fs)
        {
            _fs = fs;

            Assemblies = new FilterCollection<ProjectAssembly>();
            IgnoreSystemAssemblies = true;
            IgnorePackageFolder = true;
            IncludeDebugAssemblies = true;
            IncludeReleaseAssemblies = false;
        }


        public static readonly string[] AssemblyBlackList =
        {
            "System.",
            "Microsoft.JScript",
            "Microsoft.Build",
            "Microsoft.Activities",
            "Microsoft.CSharp",
            "Microsoft.VisualBasic",
            "Microsoft.Win32",
            "Xamarin.Forms",
            "Xamarin.Android",
            "Mono."
        };


        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (value == _searchText) return;
                _searchText = value;
                OnPropertyChanged();

                if (IsReady) ApplyFilters();
            }
        }


        public bool IgnoreSystemAssemblies
        {
            get { return _ignoreSystemAssemblies; }
            set
            {
                if (value == _ignoreSystemAssemblies) return;
                _ignoreSystemAssemblies = value;
                OnPropertyChanged();

                if (IsReady) ApplyFilters();
            }
        }


        public bool IgnorePackageFolder
        {
            get { return _ignorePackageFolder; }
            set
            {
                if (value == _ignorePackageFolder) return;
                _ignorePackageFolder = value;
                OnPropertyChanged();

                if (IsReady) ApplyFilters();
            }
        }


        public bool IncludeDebugAssemblies
        {
            get { return _includeDebugAssemblies; }
            set
            {
                if (value == _includeDebugAssemblies) return;
                _includeDebugAssemblies = value;
                OnPropertyChanged();

                if (IsReady) ApplyFilters();
            }
        }


        public bool IncludeReleaseAssemblies
        {
            get { return _includeReleaseAssemblies; }
            set
            {
                if (value == _includeReleaseAssemblies) return;
                _includeReleaseAssemblies = value;
                OnPropertyChanged();

                if (IsReady) ApplyFilters();
            }
        }


        public void Initialize(IEnumerable<ProjectAssembly> tracking)
        {
            try
            {
                var pas = tracking as ProjectAssembly[] ?? tracking.ToArray();
                var opas = pas.OrderBy(a => a.FullPath).ThenBy(a => a.RelativePath).ToArray();
                var projFolder = ToolboxApp.Project.State.ProjectFolder;
                if (string.IsNullOrWhiteSpace(projFolder)) return;

                Assemblies.Clear();

                // Assemblies.AddRange() causes exception?
                foreach (var pa in opas)
                {
                    Assemblies.Add(pa);
                }

                // find assemblies in project folder
                var comparer = new ProjectAssemblyComparer();
                var allFiles = Directory.GetFiles(projFolder, $"*{SupportedFileTypes.AssemblyExtension}", SearchOption.AllDirectories);
                var found = new List<ProjectAssembly>();

                foreach (var file in allFiles)
                {
                    var rp = file.Replace(projFolder, string.Empty);

                    var assembly = new ProjectAssembly
                    {
                        FullPath = file,
                        RelativePath = rp,
                        FileName = Path.GetFileName(file),
                        Directory = Path.GetDirectoryName(rp)
                    };

                    found.Add(assembly);
                }

                var orderedFound = found.OrderBy(a => a.FullPath).ThenBy(a => a.RelativePath);

                // add assemblies in project folder
                foreach (var assembly in orderedFound)
                {
                    if (!Assemblies.Contains(assembly, comparer))
                    {
                        Assemblies.Add(assembly);
                    }
                }

                // check if project file exists and include assemblies
                if (_fs.FileExist(ToolboxApp.Project.State.XenProjectFilePath))
                {
                    var proj = new XenProjectFile(_fs);
                    var loaded = proj.Load(ToolboxApp.Project.State.XenProjectFilePath);

                    if (!loaded)
                    {
                        ToolboxApp.Log.Warn($"Unable to load project file: {ToolboxApp.Project.State.XenProjectFilePath}.");
                    }

                    if (proj.Assemblies != null)
                    {
                        foreach (var pa in proj.Assemblies)
                        {
                            var fp = $"{ToolboxApp.Project.State.XenProjectFilePath}{pa.RelativePath}";
                            if (string.IsNullOrWhiteSpace(fp)) continue;

                            var match = Assemblies.FirstOrDefault(a => a.FullPath == fp);
                            if (match != null)
                            {
                                match.Include = true;
                            }
                        }
                    }
                }
                else
                {
                    ToolboxApp.Log.Info($"{ToolboxApp.Project.State.XenProjectFilePath} doesn't exist.");
                }
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, "Error retrieving project assemblies.");
            }
        }


        public bool UploadAssemblies()
        {
            if (IncludedAssemblies == null) return false;

            foreach (var assembly in IncludedAssemblies.Where(a => !a.Sent))
            {
                var data = File.ReadAllBytes(assembly.FullPath);
                var b64 = Convert.ToBase64String(data);

                var req = XenMessage.Create<LoadProjectRequest>();
                req.AssemblyData = b64;

                ToolboxApp.SocketManager.Socket.Send(req);
                assembly.Sent = true;
            }

            return true;
        }


        public void ApplyFilters()
        {
            Assemblies.Filter = pa => pa.Include || (!ShouldIgnore(pa.FullPath) && (SearchText == null || pa.RelativePath.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
        }


        public void Save()
        {
            try
            {
                var proj = new XenProjectFile(_fs)
                {
                    Schema = XenFormsEnvironment.ProjectFileSchema,
                    Assemblies = IncludedAssemblies?.ToArray() ?? new ProjectAssembly[] {}
                };

                proj.Save(ToolboxApp.Project.State.XenProjectFilePath);
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, $"Error saving file: {ToolboxApp.Project.State.XenProjectFilePath}.");

                MessageBox.Show(Application.Instance.MainForm,
                    "There was an error saving the project file. Please check the log for more information.",
                    "XenForms",
                    MessageBoxButtons.OK,
                    MessageBoxType.Error);
            }
        }


        private bool ShouldIgnore(string fullPath)
        {
            if (String.IsNullOrWhiteSpace(fullPath)) return true;

            var fileName = Path.GetFileName(fullPath);
            var folders = fullPath.ToLower(CultureInfo.CurrentCulture).Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            if (IgnorePackageFolder && (folders.Contains("packages") || folders.Contains("components")))
            {
                return true;
            }

            if (folders.Contains("obj"))
            {
                return true;
            }

            if (!IncludeDebugAssemblies && folders.Contains("debug"))
            {
                return true;
            }

            if (!IncludeReleaseAssemblies && folders.Contains("release"))
            {
                return true;
            }

            if (IgnoreSystemAssemblies)
            {
                foreach (var blocked in AssemblyBlackList)
                {
                    if (fileName.StartsWith(blocked))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}