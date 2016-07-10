using System;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Toolbox.UI.Pouches.FilesFolder.Commands
{
    public class HomeButtonCommand
    {
        public static void Execute(string folder)
        {
            try
            {
                var dlg = MessageBox.Show(Application.Instance.MainForm,
                    "Are you sure you want to set the opened folder as the default folder?",
                    "XenForms",
                    MessageBoxButtons.YesNo,
                    MessageBoxType.Question);

                if (dlg == DialogResult.No) return;

                ToolboxApp.Settings.Set(UserSettingKeys.Builtin.DefaultProjectDirectory, folder);
                ToolboxApp.Bus.Notify(new ShowStatusMessage("New default directory set."));
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, "There was an error setting the default directory.");

                MessageBox.Show(Application.Instance.MainForm,
                    "There was an error setting the default directory. Please see the logs.",
                    "XenForms",
                    MessageBoxButtons.OK,
                    MessageBoxType.Error);
            }
        }
    }
}