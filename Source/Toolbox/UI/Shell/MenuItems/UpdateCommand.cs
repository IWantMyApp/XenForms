using System;
using System.ComponentModel;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using Squirrel;
using XenForms.Core;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class UpdateCommand : Command
    {
        #if DEBUG
        private const string UpdateUri = @"D:\mikedav.is\ws\XenForms\Build\Releases";
        #else
        private const string UpdateUri = @"http://releases.xenforms.com/";
        #endif

        private ProgressBar _progress;

        private Button _escape;
        private Button _updateBtn;

        private Label _currentLabel;
        private Label _statusLabel;

        private Dialog _dlg;
        private bool _updating;


        public UpdateCommand()
        {
            MenuText = "Check for updates...";
        }


        internal static string GetUpdateUri()
        {
            var uri = ToolboxApp.Settings.GetString(UserSettingKeys.Builtin.UpdateChannel);

            if (uri == null)
            {
                uri = UpdateUri;
                ToolboxApp.Settings.Set(UserSettingKeys.Builtin.UpdateChannel, uri);
            }

            return uri;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            _progress = new ProgressBar
            {
                MinValue = 0,
                MaxValue = 100,
                Indeterminate = true
            };

            var footer = new TableLayout(3, 1)
            {
                Padding = new Padding(0, 10, 0, 0),
                Spacing = new Size(5,0)
            };

            _updateBtn = new Button {Text = "Update", Enabled = false};
            _escape = new Button {Text = "Cancel"};

            footer.Add(null, 0, 0, true, false);
            footer.Add(_updateBtn, 1, 0, false, false);
            footer.Add(_escape, 2, 0, false, false);

            _currentLabel = new Label
            {
                Text = $"Checking for updates.\nCurrently running {XenFormsEnvironment.ToolboxVersion}.",
                VerticalAlignment = VerticalAlignment.Center,
                Wrap = WrapMode.Word
            };

            _statusLabel = new Label
            {
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };

            var currentItem = new StackLayoutItem(_currentLabel, VerticalAlignment.Center, true)
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var stack = new StackLayout
            {
                Spacing = 5,
                Orientation = Orientation.Vertical
            };

            stack.Items.Add(currentItem);
            stack.Items.Add(new StackLayoutItem(_statusLabel, HorizontalAlignment.Stretch));
            stack.Items.Add(new StackLayoutItem(_progress, HorizontalAlignment.Stretch));
            stack.Items.Add(new StackLayoutItem(footer, HorizontalAlignment.Stretch));

            _dlg = new Dialog
            {
                Title = "Update Manager",
                Padding = new Padding(10),
                Content = stack,
                Size = new Size(300,200),
                Icon = AppImages.Xf,
                AbortButton = _escape
            };

            _updateBtn.Click += OnUpdate;
            _escape.Click += OnEscape;

            _dlg.Shown += OnShown;
            _dlg.Closing += OnDialogClosing;
            _dlg.ShowModal(Application.Instance.MainForm);
        }


        private void OnDialogClosing(object sender, CancelEventArgs e)
        {
            if (!_updating)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;

                MessageBox.Show(Application.Instance.MainForm, "You cannot close this dialog while updating.",
                    "XenForms", MessageBoxButtons.OK);
            }
        }


        private void OnUpdate(object sender, EventArgs e)
        {
            try
            {
                Application.Instance.AsyncInvoke(async () =>
                {
                    _updating = true;
                    _progress.Indeterminate = true;
                    _statusLabel.Text = "Working...";
                    _updateBtn.Enabled = false;
                    _escape.Enabled = false;

                    using (var mgr = new UpdateManager(GetUpdateUri()))
                    {
                        var result = await mgr.UpdateApp();
                        _statusLabel.Text = $"{result.Version} installed successfully. Please restart the application.";
                    }

                    _updating = false;
                    _escape.Text = "Ok";
                    _escape.Enabled = true;
                    _progress.Indeterminate = false;
                    _progress.Value = 100;
                });
            }
            catch (Exception ex)
            {
                _updating = false;
                _dlg.Close();

                ToolboxApp.Log.Error(ex, "Error updating application.");
                MessageBox.Show(Application.Instance.MainForm, "An error occurred while updating the application. Please check the log viewer for more information.",
                    "XenForms", MessageBoxType.Error);
            }
        }


        private void OnEscape(object sender, EventArgs e)
        {
            _dlg.Close();
        }


        private async void OnShown(object sender, EventArgs e)
        {
            try
            {
                using (var mgr = new UpdateManager(UpdateUri))
                {
                    var result = await mgr.CheckForUpdate();

                    _progress.Indeterminate = false;
                    _progress.Value = 100;

                    if (result.ReleasesToApply.Any())
                    {
                        _statusLabel.Text = "You have updates to apply. Save all work before continuing, and then click update.";
                        _updateBtn.Enabled = true;
                    }
                    else
                    {
                        _statusLabel.Text = "You are running the most recent version.";
                        _updateBtn.Enabled = false;
                        _updating = false;
                        _escape.Text = "Ok";
                    }
                }
            }
            catch (Exception ex)
            {
                _dlg.Close();

                ToolboxApp.Log.Error(ex, "Error checking for updates.");
                MessageBox.Show(Application.Instance.MainForm, "An error occurred while updating the application. Please check the log viewer for more information.",
                    "XenForms", MessageBoxType.Error);
            }
        }
    }
}
