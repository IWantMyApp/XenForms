using System;
using System.IO;
using System.Text;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Toolbox.Project;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class SaveXamlCommand : Command
    {
        public string XenExtensionMessage => $"You cannot overwrite existing *.xaml files. You can only save files as *{SupportedFileTypes.XenXamlExtension} in this beta build.";


        public SaveXamlCommand()
        {
            MenuText = "Save As...";
            ToolTip = "Save As...";
            Image = AppImages.Save;

            ToolboxApp.Bus.Listen<SaveXamlReceived>(e =>
            {
                Application.Instance.Invoke(() =>
                {
                    OnSaveXamlMessageReceived(e);
                });
            });
        }


        public override MenuItem CreateMenuItem()
        {
            var item = base.CreateMenuItem();

            item.Validate += (sender, args) =>
            {
                item.Enabled = ToolboxApp.SocketManager.IsConnected;
            };

            return item;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            try
            {
                ToolboxApp.Log.Info("Sending save request to designer.");
                ToolboxApp.Designer.SaveXaml();
                WorkingDialog.Show();
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, "Exception occurred while saving XAML.");
            }
            finally
            {
                ToolboxApp.Log.Info("Save dialog closed.");
            }
        }


        private string GetOutput(string input)
        {
            ToolboxApp.XamlProcessor.LoadDocument(input, VisualTree.XamlElements);
            var doc = ToolboxApp.XamlProcessor.Process();
            var output = doc.ToString();
            return output;
        }


        private DialogResult XenFormsExtensionAcceptance(MessageBoxType type = MessageBoxType.Information)
        {
            return MessageBox.Show(Application.Instance.MainForm, XenExtensionMessage, "XenForms", type);
        }


        private bool ShowSaveDialog(string output)
        {
            try
            {
                var saveDlg = new SaveFileDialog
                {
                    Title = "Save As..."
                };

                saveDlg.Filters.Add(new FileDialogFilter("XenForms XAML", SupportedFileTypes.XenXamlExtension));
                var dlgResult = saveDlg.ShowDialog(Application.Instance.MainForm);

                if (dlgResult == DialogResult.Ok)
                {
                    if (string.IsNullOrWhiteSpace(saveDlg.FileName)) return false;

                    if (saveDlg.FileName.EndsWith(SupportedFileTypes.XenXamlExtension))
                    {
                        File.WriteAllText(saveDlg.FileName, output, Encoding.UTF8);
                    }
                    else
                    {
                        XenFormsExtensionAcceptance(MessageBoxType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, "Exception occurred while saving XAML.");
                return false;
            }

            return true;
        }


        private void OnSaveXamlMessageReceived(SaveXamlReceived e)
        {
            string xaml = null;

            try
            {
                xaml = GetOutput(e.Xaml);
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, "Error processing XAML.");

                MessageBox.Show(Application.Instance.MainForm,
                    "Error processing XAML before saving. Please see error log for more details.",
                    "XenForms",
                    MessageBoxType.Error);
            }
            finally
            {
                WorkingDialog.Close();
            }

            if (!string.IsNullOrWhiteSpace(xaml))
            {
                XenFormsExtensionAcceptance();

                var succeeded = ShowSaveDialog(xaml);

                if (!succeeded)
                {
                    MessageBox.Show(Application.Instance.MainForm,
                        "The save operation failed. Please review the log for more information.",
                        "XenForms",
                        MessageBoxType.Error);
                }
            }
        }
    }
}