using System;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Toolbox.UI.Shell;
using XenForms.Toolbox.UI.Shell.Images;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Pouches.Properties
{
    public class RegisterNewTypeDialog
    {
        private readonly ConnectedDialog _dlg;
        private readonly TextBox _textBox;


        public RegisterNewTypeDialog()
        {
            var desc = new Label
            {
                Text = "Adding a supported type will allow XenForms to recognize additional view properties. You may need to use the Load Project Assemblies Dialog, first. Only value types are supported in the Beta. \n\ne.g. Xamarin.Forms.Rectangle"
            };

            _textBox = new TextBox();

            var container = new TableLayout(1, 3)
            {
                Spacing = new Size(0, 10)
            };

            var footer = new TableLayout(3, 1)
            {
                Spacing = new Size(5, 0)
            };

            var ok = new Button { Text = "Ok" };
            var cancel = new Button { Text = "Cancel" };

            footer.Add(null, 0, 0, true, false);
            footer.Add(cancel, 1, 0, false, false);
            footer.Add(ok, 2, 0, false, false);

            container.Add(desc, 0, 0, true, true);
            container.Add(_textBox, 0, 1, false, false);
            container.Add(footer, 0, 2, false, false);

            _dlg = new ConnectedDialog
            {
                Padding = AppStyles.WindowPadding,
                Content = container,
                Width = 350,
                Height = 220,
                Icon = AppImages.Xf,
                Title = "Register New Property Type",
                AbortButton = cancel,
                DefaultButton = ok
            };

            ok.Click += OnClick;
            cancel.Click += (s, e) => Close();
        }


        public void Show()
        {
            ToolboxApp.Bus.Listen<AddSupportedTypeEvent>(args =>
            {
                Application.Instance.Invoke(() =>
                {
                    MessageBox.Show(_dlg,
                        args.DisplayMessage,
                        "XenForms",
                        MessageBoxButtons.OK);

                    if (args.Added || args.AlreadyRegistered)
                    {
                        Close();
                    }
                });
            });

            _dlg.ShowModal(Application.Instance.MainForm);
        }


        public void Close()
        {
            ToolboxApp.Bus.StopListening<AddSupportedTypeEvent>();
            _dlg.Close();
        }


        private void OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_textBox.Text)) return;

            ToolboxApp.Project.AddSupportedType(_textBox.Text);
        }
    }
}