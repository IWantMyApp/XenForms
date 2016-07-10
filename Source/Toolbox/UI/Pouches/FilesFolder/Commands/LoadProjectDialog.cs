using System;
using System.Linq;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Shell;
using XenForms.Toolbox.UI.Shell.Images;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Pouches.FilesFolder.Commands
{
    public class LoadProjectDialog
    {
        private readonly LoadProjectDialogModel _model;
        private ConnectedDialog _dlg;
        private GridView _grid;
        private SearchBox _search;


        public LoadProjectDialog(LoadProjectDialogModel model)
        {
            _model = model;
        }


        public async Task ShowAsync()
        {
            _model.Initialize(ToolboxApp.Project.UserAssemblies);

            var ok = new Button { Text = "Ok" };
            var close = new Button { Text = "Close" };

            var container = new TableLayout(1, 7)
            {
                Spacing = new Size(0, 10)
            };

            _dlg = new ConnectedDialog
            {
                DataContext = _model,
                Icon = AppImages.Xf,
                Title = "Load Project Assemblies",
                Padding = AppStyles.WindowPadding,
                MinimumSize = new Size(775, 500),
                Width = 775,
                Height = 500,
                AbortButton = close,
                Content = container,
                Resizable = true
            };

            var searchbar = new TableLayout(2, 1)
            {
                Spacing = new Size(10, 0)
            };

            var footer = new TableLayout(3, 1)
            {
                Spacing = new Size(5,0)
            };

            var options1 = new StackLayout
            {
                Spacing = 5,
                Orientation = Orientation.Horizontal
            };

            var options2 = new StackLayout
            {
                Spacing = 5,
                Orientation = Orientation.Horizontal
            };

            _search = new SearchBox
            {
                Width = 300,
                PlaceholderText = "Search"
            };

            var instructions = new Label
            {
                Text = "Select any project assemblies that contain custom renderers, custom page types, etc. Any assembly that contains types that are required for your XAML to load correctly should be selected."
            };

            var recommend = new Label
            {
                Text = "* It's recommended to select debug assemblies.\n* Selecting assemblies will create a project.xen.json file in your project's folder."
            };

            var ignoreSystem = new CheckBox
            {
                Width = 200,
                Text = "Ignore system assemblies.",
            };

            var ignorePackage = new CheckBox
            {
                Text = "Ignore packages && components folder.",
            };

            var includeDebug = new CheckBox
            {
                Width = 200,
                Text = "Include debug folder.",
            };

            var includeRelease = new CheckBox
            {
                Text = "Include release folder.",
            };

            _grid = new GridView
            {
                ShowHeader = true,
                GridLines = GridLines.Horizontal
            };

            _grid.Columns.Add(new GridColumn { AutoSize = true, HeaderText = "Include", DataCell = new CheckBoxCell("Include"), Editable = true });
            _grid.Columns.Add(new GridColumn { AutoSize = true, HeaderText = "File Name", DataCell = new TextBoxCell("FileName"), Editable = false });
            _grid.Columns.Add(new GridColumn { AutoSize = true, HeaderText = "Directory", DataCell = new TextBoxCell("Directory"), Editable = false });
            _grid.Columns.Add(new GridColumn { AutoSize = true, HeaderText = "Loaded", DataCell = new TextBoxCell("Sent"), Editable = false });

            searchbar.Add(_search, 0, 0, false, false);
            searchbar.Add(null, 1, 0, true, false);

            options1.Items.Add(ignoreSystem);
            options1.Items.Add(ignorePackage);
            options2.Items.Add(includeDebug);
            options2.Items.Add(includeRelease);

            footer.Add(null, 0, 0, true, false);
            footer.Add(close, 1, 0, false, false);
            footer.Add(ok, 2, 0, false, false);

            container.Add(instructions, 0, 0, true, false);
            container.Add(searchbar, 0, 1, true, false);
            container.Add(_grid, 0, 2, true, true);
            container.Add(options1, 0, 3, true, false);
            container.Add(options2, 0, 4, true, false);
            container.Add(recommend, 0, 5, true, false);
            container.Add(footer, 0, 6, true, false);

            var nothingFound = new Label
            {
                Text = "No assemblies found in your project folder."
            };

            if (_model.Assemblies == null || _model.Assemblies.Count == 0)
            {
                searchbar.Add(nothingFound, 1, 0, true, false);
            }

            _grid.DataStore = _model.Assemblies;
            _search.TextBinding.BindDataContext((LoadProjectDialogModel m) => m.SearchText);
            includeRelease.CheckedBinding.BindDataContext((LoadProjectDialogModel m) => m.IncludeReleaseAssemblies);
            includeDebug.CheckedBinding.BindDataContext((LoadProjectDialogModel m) => m.IncludeDebugAssemblies);
            ignorePackage.CheckedBinding.BindDataContext((LoadProjectDialogModel m) => m.IgnorePackageFolder);
            ignoreSystem.CheckedBinding.BindDataContext((LoadProjectDialogModel m) => m.IgnoreSystemAssemblies);

            _dlg.LoadComplete += (s, e) =>
            {
                _model.ApplyFilters();
                _model.IsReady = true;
            };

            ok.Click += (s, e) => Ok();
            close.Click += (s, e) => Close();
            await _dlg.ShowModalAsync(Application.Instance.MainForm);
        }


        private void Ok()
        {
            try
            {
                ToolboxApp.Project.AddAssemblies(_model.IncludedAssemblies);

                if (_model.IncludedAssemblies != null && _model.IncludedAssemblies.Any())
                {
                    Application.Instance.AsyncInvoke(WorkingDialog.Show);
                    _model.UploadAssemblies();
                    Application.Instance.AsyncInvoke(WorkingDialog.Close);
                }

                _model.Save();

                Close();
            }
            catch (Exception e)
            {
                ToolboxApp.Log.Error(e, "Error sending project assemblies to designer.");

                MessageBox.Show(Application.Instance.MainForm,
                    "Unable to send project assemblies to the designer. Review the log for more information.",
                    MessageBoxButtons.OK, MessageBoxType.Error);
            }
        }


        public void Close()
        {
            _dlg.Close();
        }
    }
}