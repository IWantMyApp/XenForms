using System;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Toolbox.UI.Pouches.FilesFolder.Commands
{
    public class OpenXamlModel
    {
        public static void Open(string selectedFile)
        {
            try
            {
                if (ToolboxApp.FileSystem.FileExist(selectedFile))
                {
                    string xaml;
                    var found = ToolboxApp.FileSystem.ReadAllText(selectedFile, out xaml);

                    if (found)
                    {
                        ToolboxApp.Designer.OpenXaml(xaml, selectedFile);
                        ToolboxApp.Bus.Notify(new ShowStatusMessage($"Editing XAML file: {selectedFile ?? "Unknown. See Log."}"));
                    }
                }
                else
                {
                    ToolboxApp.Log.Warn($"XAML file doesn't exist: {selectedFile}.");
                    ToolboxApp.Bus.Notify(new ShowStatusMessage($"Failed to open: {selectedFile}"));
                }
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, "An error occurred while opening the XAML file for editing.");

                MessageBox.Show(Application.Instance.MainForm,
                    "An error occurred while opening the XAML file for editing. Please see the log viewer for more information.",
                    MessageBoxType.Error);
            }
        }
    }
}