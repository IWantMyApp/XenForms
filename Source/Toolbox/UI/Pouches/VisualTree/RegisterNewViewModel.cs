using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Eto.Forms;
using XenForms.Core;
using XenForms.Core.FileSystem;
using XenForms.Core.Platform;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Toolbox.Project;
using XenForms.Core.Toolbox.Shell;
using XenForms.Toolbox.UI.Properties;

namespace XenForms.Toolbox.UI.Pouches.VisualTree
{
    public class RegisterNewViewModel : INotifyPropertyChanged
    {
        private readonly IFileSystem _fs;
        private string _typeName;
        private ProjectView _selectedItem;
        private bool _deleteEnabled;
        private bool _addEnabled;

        public event EventHandler<RecreateMenuItemsEventArgs> RecreateMenuItems;

        public FilterCollection<ProjectView> Views { get; }
        public IEnumerable<ProjectView> Persistable => Views?.Where(v => !string.IsNullOrWhiteSpace(v.TypeName) && !v.Deleted);
        public VisualTreeNode SelectedNode { get; private set; }


        public bool DeleteEnabled
        {
            get { return _deleteEnabled; }
            set
            {
                if (value == _deleteEnabled) return;
                _deleteEnabled = value;
                OnPropertyChanged();
            }
        }


        public bool AddEnabled
        {
            get { return _addEnabled; }
            set
            {
                if (value == _addEnabled) return;
                _addEnabled = value;
                OnPropertyChanged();
            }
        }


        public ProjectView SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (Equals(value, _selectedItem)) return;
                _selectedItem = value;

                DeleteEnabled = value != null;
                OnPropertyChanged();
            }
        }


        public string TypeName
        {
            get { return _typeName; }
            set
            {
                if (value == _typeName) return;
                _typeName = value;

                AddEnabled = !string.IsNullOrWhiteSpace(TypeName);
                var match = Views?.Any(v => v.TypeName == TypeName);

                if (match != null)
                {
                    if (match.Value)
                    {
                        AddEnabled = false;
                    }
                }

                OnPropertyChanged();
            }
        }


        public RegisterNewViewModel(IFileSystem fs, IMessageBus bus)
        {
            _fs = fs;
            Views = new FilterCollection<ProjectView> {Sort = Sort};

            bus.Listen<VisualTreeNodeSelected>(payload =>
            {
                SelectedNode = payload.Node;
            });

            ApplyFilters();
        }


        public void AddView()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TypeName)) return;

                var match = Views.FirstOrDefault(v => v.TypeName == TypeName);
                if (match != null) return;

                var pv = new ProjectView
                {
                    Deleted = false,
                    Persisted = false,
                    TypeName = TypeName
                };

                Views.Add(pv);
                TypeName = String.Empty;
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, $"Unable to add new view: {TypeName}.");
            }
        }


        public bool Initialize()
        {
            try
            {
                var proj = new XenProjectFile(_fs);
                var loaded = proj.Load(ToolboxApp.Project.State.XenProjectFilePath);

                if (loaded == false || proj.Views == null)
                {
                    OnRecreateMenuItems(new RecreateMenuItemsEventArgs(null));
                }

                if (!loaded) return false;
                if (proj.Views == null) return false;

                var sorted = proj.Views.OrderBy(v => v.TypeName).ToArray();
                OnRecreateMenuItems(new RecreateMenuItemsEventArgs(sorted));
                return true;
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, $"Error initializing {nameof(RegisterNewViewModel)}.");
                return false;
            }
        }


        public bool Show()
        {
            try
            {
                var proj = new XenProjectFile(_fs);
                var loaded = proj.Load(ToolboxApp.Project.State.XenProjectFilePath);

                if (!loaded) return false;
                if (proj.Views == null) return false;

                // clear the views after when know that we can save some output
                Views.Clear();
                var sorted = proj.Views.OrderBy(v => v.TypeName).ToArray();

                foreach (var pv in sorted)
                {
                    pv.Deleted = false;
                    pv.Persisted = true;
                    Views.Add(pv);
                }

                OnRecreateMenuItems(new RecreateMenuItemsEventArgs(proj.Views));
                return true;
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, $"Error showing {nameof(RegisterNewViewModel)}.");
                return false;
            }
        }


        public bool DiscardChanges()
        {
            try
            {
                Views.Filter = null;
                SelectedItem = null;
                TypeName = String.Empty;

                var remove = Views.Where(v => !v.Persisted).ToArray();

                foreach (var view in remove)
                {
                    Views.Remove(view);
                }

                foreach (var view in Views)
                {
                    view.Deleted = false;
                }

                ApplyFilters();
                return true;
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, $"Error discarding changes in {nameof(RegisterNewViewModel)}.");
                return false;
            }
        }


        public bool Delete()
        {
            try
            {
                if (Views.Count == 0) return false;
                if (SelectedItem == null) return false;

                var match = Views.FirstOrDefault(v => v.TypeName == SelectedItem.TypeName);
                if (match == null) return false;
                match.Deleted = true;

                ApplyFilters();
                return true;
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, "Error removing selected view.");
                return false;
            }
        }


        public bool Save()
        {
            try
            {
                var persistable = Persistable.ToArray();

                if (string.IsNullOrWhiteSpace(ToolboxApp.Project.State.ProjectFolder))
                {
                    OnRecreateMenuItems(new RecreateMenuItemsEventArgs(persistable));
                    return false;
                }

                var proj = new XenProjectFile(_fs);
                var loaded = proj.Load(ToolboxApp.Project.State.XenProjectFilePath);

                if (!loaded)
                {
                    ToolboxApp.Log.Info($"Creating new project file: {ToolboxApp.Project.State.XenProjectFilePath}.");
                    proj.Schema = XenFormsEnvironment.ProjectFileSchema;
                }

                proj.Views = persistable;
                proj.Save(ToolboxApp.Project.State.XenProjectFilePath);

                foreach (var pv in proj.Views)
                {
                    pv.Persisted = true;
                }

                OnRecreateMenuItems(new RecreateMenuItemsEventArgs(proj.Views));
                return true;
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, $"Error saving file: {ToolboxApp.Project.State.XenProjectFilePath}.");

                MessageBox.Show(Application.Instance.MainForm,
                    "There was an error saving the project file. Please check the log for more information.",
                    "XenForms",
                    MessageBoxButtons.OK,
                    MessageBoxType.Error);

                return false;
            }
        }


        private void ApplyFilters()
        {
            Views.Filter = v => !v.Deleted;
        }


        private int Sort(ProjectView v1, ProjectView v2)
        {
            return String.Compare(v1.TypeName, v2.TypeName, StringComparison.Ordinal);
        }


        protected virtual void OnRecreateMenuItems(RecreateMenuItemsEventArgs e)
        {
            RecreateMenuItems?.Invoke(this, e);
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
