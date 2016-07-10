using Eto.Drawing;
using Eto.Forms;
using XenForms.Toolbox.UI.Logging;
using XenForms.Toolbox.UI.Resources;
using XenForms.Toolbox.UI.Shell.Images;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Shell
{
    public static class ErrorDialog
    {
        public static Dialog Create(string message)
        {
            var dialog = new Dialog
            {
                Icon = AppImages.Xf,
                Title = AppResource.Error_dialog_title,
                Padding = AppStyles.WindowPadding,
                Width = 500,
                Height = 400,
            };

            var container = new TableLayout(1, 3)
            {
                Spacing = new Size(0, 10)
            };

            var about = new Label { Text = AboutResource.Save_log_before_termination };
            var txtCtrl = new TextArea { ReadOnly = true, Text = message, Wrap = true };
            var ok = new Button { Text = CommonResource.Ok };
            var export = new LinkButton {Text = AboutResource.Export_log_and_trace};

            export.Click += (s, e) => { SaveLogDialog.Show(message); };

            container.Add(about, 0, 0, true, false);
            container.Add(txtCtrl, 0, 1, true, true);

            var btnRow = new TableLayout(3, 1) {Spacing = new Size(6, 0)};

            btnRow.Add(export, 0, 0, false, false);
            btnRow.Add(null, 1, 0, true, false);
            btnRow.Add(ok, 2, 0, false, false);

            container.Add(btnRow, 0, 2, true, false);

            dialog.Content = container;
            ok.Click += (s, e) => dialog.Close();

            return dialog;
        }
    }
}