using System.IO;
using Eto.Forms;
using XenForms.Core.Toolbox;

namespace XenForms.Toolbox.UI.Logging
{
    public static class SaveLogDialog
    {
        public static void Show(string extra = null)
        {
            var dialog = new SaveFileDialog();
            dialog.Filters.Add(new FileDialogFilter("Log File", ".txt"));
            var result = dialog.ShowDialog(Application.Instance.MainForm);

            if (result == DialogResult.Ok)
            {
                var logs = ToolboxApp.Log.GetDetailedLogInformation(extra);
                File.WriteAllLines(dialog.FileName, logs);
            }
        }
    }
}