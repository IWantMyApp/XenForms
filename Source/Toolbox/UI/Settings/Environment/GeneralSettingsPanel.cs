using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.Settings;
using XenForms.Core.Toolbox.Shell;
using XenForms.Toolbox.UI.Shell;

namespace XenForms.Toolbox.UI.Settings.Environment
{
    [SettingsPanel(SettingsPanelAttribute.Builtin.Environment, SettingsPanelAttribute.Builtin.General)]
    public class GeneralSettingsPanel : XenForm
    {
        private readonly ISettingsStore _store;
        private readonly GeneralSettingsPanelModel _vm;


        public GeneralSettingsPanel(ISettingsStore store)
        {
            _store = store;
            _vm = new GeneralSettingsPanelModel();
        }


        protected override Control OnDefineLayout()
        {
            var layout = new TableLayout(1, 4) {DataContext = _vm};

            layout.Add(CreateAppearanceGroup(), 0, 0, true, false);
            layout.Add(CreatePathsGroup(), 0, 1, true, false);
            layout.Add(CreateLoggingGroup(), 0, 2, true, false);
            layout.Add(null, 0, 3, true, true);

            return layout;
        }


        protected override async Task OnShowFormAsync(object data = null)
        {
            await base.OnShowFormAsync(data);

            _vm.ShowConnectOnStartup = _store.GetBool(UserSettingKeys.Builtin.ShowConnectOnStart) ?? false;
            _vm.EnableTraceLogging = _store.GetBool(UserSettingKeys.Builtin.TraceLoggingEnabled) ?? false;
            _vm.DefaultProjectDirectory = _store.GetString(UserSettingKeys.Builtin.DefaultProjectDirectory);
            _vm.XamarinStudioDirectory = _store.GetString(UserSettingKeys.Builtin.XamarinStudioDirectory);
            _vm.AdbLocation = _store.GetString(UserSettingKeys.Builtin.AdbLocation);
            _vm.OriginalAdbLocation = _vm.AdbLocation;
        }


        protected override async Task OnCloseFormAsync(bool save = false)
        {
            await base.OnCloseFormAsync(save);

            if (!save) return;

            _store.Set(UserSettingKeys.Builtin.ShowConnectOnStart, _vm.ShowConnectOnStartup);
            _store.Set(UserSettingKeys.Builtin.TraceLoggingEnabled, _vm.EnableTraceLogging);
            _store.Set(UserSettingKeys.Builtin.DefaultProjectDirectory, _vm.DefaultProjectDirectory);
            _store.Set(UserSettingKeys.Builtin.XamarinStudioDirectory, _vm.XamarinStudioDirectory);
            _store.Set(UserSettingKeys.Builtin.AdbLocation, _vm.AdbLocation);

            ToolboxApp.Log.ConfigureTraceLog(_store);

            if (_vm.ShowRestartMessage)
            {
                MessageBox.Show(Application.Instance.MainForm,
                    "You will need to restart the application for the changes to take effect.",
                    "XenForms",
                    MessageBoxButtons.OK);
            }
        }


        private Control CreateLoggingGroup()
        {
            var group = new GroupBox { Text = "Logging" };
            var layout = new TableLayout(1, 3);

            var enableTrace = new CheckBox { Text = "Enable trace level" };
            enableTrace.Bind(c => c.Checked, _vm, v => v.EnableTraceLogging);

            layout.Add(enableTrace, 0, 0, true, false);
            layout.Rows.Add(null);

            group.Content = layout;
            return group;
        }


        private Control CreateAppearanceGroup()
        {
            var group = new GroupBox { Text = "Appearance" };
            var layout = new TableLayout(1, 3);

            var connectOnStartup = new CheckBox { Text = "Show connection dialog on application startup" };
            connectOnStartup.Bind(c => c.Checked, _vm, v => v.ShowConnectOnStartup);

            layout.Add(connectOnStartup, 0, 0, true, false);
            layout.Rows.Add(null);

            group.Content = layout;
            return group;
        }


        private Control CreatePathsGroup()
        {
            var group = new GroupBox {Text = "Paths"};

            var layout = new TableLayout(1, 3)
            {
                Spacing = new Size(0, 5)
            };

            layout.Add(CreateHomeDirectory(), 0, 0, true, false);
            layout.Add(CreateXamarinStudioDirectory(), 0, 1, true, false);
            layout.Add(CreateAdbLocation(), 0, 2, true, false);

            group.Content = layout;
            return group;
        }
        

        private Control CreateHomeDirectory()
        {
            var layout = new TableLayout(2, 2);

            var home = new Label { Text = "Project Files Home Directory" };
            var txt = new TextBox();
            var btn = new Button { Text = "Select" };

            txt.Bind(b => b.Text, _vm, b => b.DefaultProjectDirectory);

            btn.Click += (s, e) =>
            {
                var dialog = new SelectFolderDialog();
                var result = dialog.ShowDialog(Application.Instance.MainForm);

                if (result == DialogResult.Ok)
                {
                    _vm.DefaultProjectDirectory = dialog.Directory;
                }
            };

            layout.Spacing = new Size(5, 5);
            layout.Add(home, 0, 0, true, false);
            layout.Add(txt, 0, 1, true, false);
            layout.Add(btn, 1, 1, false, false);

            return layout;
        }


        private Control CreateXamarinStudioDirectory()
        {
            var layout = new TableLayout(2, 2);

            var home = new Label { Text = "Xamarin Studio Bin Directory" };
            var txt = new TextBox();
            var btn = new Button { Text = "Select" };

            txt.Bind(b => b.Text, _vm, b => b.XamarinStudioDirectory);

            btn.Click += (s, e) =>
            {
                var dialog = new SelectFolderDialog();
                var result = dialog.ShowDialog(Application.Instance.MainForm);

                if (result == DialogResult.Ok)
                {
                    _vm.XamarinStudioDirectory = dialog.Directory;
                }
            };

            layout.Spacing = new Size(5, 5);
            layout.Add(home, 0, 0, true, false);
            layout.Add(txt, 0, 1, true, false);
            layout.Add(btn, 1, 1, false, false);

            return layout;
        }


        private Control CreateAdbLocation()
        {
            var layout = new TableLayout(2, 2);

            var lbl = new Label { Text = "Android's adb.exe Location" };
            var txt = new TextBox();

            var btn = new Button { Text = "Select" };

            txt.Bind(b => b.Text, _vm, b => b.AdbLocation);

            btn.Click += (s, e) =>
            {
                var dialog = new OpenFileDialog
                {
                    Filters =
                    {
                        new FileDialogFilter("Executable", "exe"),
                    }
                };
                
                var result = dialog.ShowDialog(Application.Instance.MainForm);

                if (result == DialogResult.Ok)
                {
                    _vm.AdbLocation = dialog.FileName;
                }
            };

            layout.Spacing = new Size(5, 5);
            layout.Add(lbl, 0, 0, true, false);
            layout.Add(txt, 0, 1, true, false);
            layout.Add(btn, 1, 1, false, false);

            return layout;
        }
    }
}