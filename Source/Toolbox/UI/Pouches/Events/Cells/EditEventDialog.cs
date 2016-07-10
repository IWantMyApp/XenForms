using System;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Platform.FileSystem;
using XenForms.Core.Reflection;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Resources;
using XenForms.Toolbox.UI.Shell;
using XenForms.Toolbox.UI.VisualStudio;

namespace XenForms.Toolbox.UI.Pouches.Events.Cells
{
    public static class EditEventDialog
    {
        public static Dialog Create(IToolboxEventModel model)
        {
            var dialog = new ConnectedDialog
            {
                Title = $"{model.DisplayName} Event",
                Padding = new Padding(10),
                Width = 400,
                Height = 390,
            };

            var directions = new Label
            {
                Text = "Select a file that will contain your event handler. If one does not exist, select the create option below."
            };

            // event handler name
            var handlerLbl = new Label {Text = "Event Handler Name"};
            var handlerTxt = new TextBox();
            var handlerLayout = new TableLayout(1, 2)
            {
                Spacing = new Size(0,5)
            };

            handlerLayout.Add(handlerLbl, 0, 0, true, false);
            handlerLayout.Add(handlerTxt, 0, 1, true, false);

            // Select or create file layout
            var selectRdo = new RadioButton {Text = "Select Project File"};
            var selectTxt = new TextBox();
            var selectBtn = new Button
            {
                Text = "Select",
            };

            var selectStack = new StackLayout
            {
                Spacing = 5,
                Orientation = Orientation.Horizontal,
                Items =
                {
                    new StackLayoutItem(selectTxt, true),
                    new StackLayoutItem(selectBtn)
                }
            };

            var createRdo = new RadioButton(selectRdo) { Text = "Create Project File" };
            var createTxt = new TextBox();
            var createBtn = new Button
            {
                Text = "Create",
            };

            var createStack = new StackLayout
            {
                Spacing = 5,
                Orientation = Orientation.Horizontal,
                Items =
                {
                    new StackLayoutItem(createTxt, true),
                    new StackLayoutItem(createBtn)
                }
            };

            var editorsLbl = new Label {Text = "Code Editors"};
            var editorsLst = new ListBox();
            editorsLst.Items.Add("Visual Studio", "VS");
            editorsLst.Items.Add("Xamarin Studio", "XS");
            editorsLst.SelectedIndex = 0;

            var fileLayout = new TableLayout(1, 6)
            {
                Spacing = new Size(0, 10)
            };
            
            // select
            fileLayout.Add(selectRdo, 0, 0, true, false);
            fileLayout.Add(selectStack, 0, 1, true, false);

            // create 
            fileLayout.Add(createRdo, 0, 2, true, false);
            fileLayout.Add(createStack, 0, 3, true, false);

            // ide choice
            fileLayout.Add(editorsLbl, 0, 4, true, false);
            fileLayout.Add(editorsLst, 0, 5, true, false);

            // button rows
            var cancel = new Button {Text = CommonResource.Cancel};
            var ok = new Button { Text = CommonResource.Ok };
            var btnRow = new TableLayout(3, 1)
            {
                Spacing = new Size(5, 0)
            };

            btnRow.Add(null, 0, 0, true, false);
            btnRow.Add(ok, 1, 0, false, false);
            btnRow.Add(cancel, 2, 0, false, false);

            // dialog main layout
            var container = new TableLayout(1, 5)
            {
                Spacing = new Size(0, 10)
            };

            container.Add(directions, 0, 0, true, false);
            container.Add(handlerLayout, 0, 1, true, false);
            container.Add(fileLayout, 0, 2, true, false);
            container.Add(null, 0, 3, true, true);

            container.Add(btnRow, 0, 4, true, false);

            dialog.Content = container;
            cancel.Click += (s, e) => dialog.Close();

            dialog.DefaultButton = cancel;

            // refactor later

            selectRdo.CheckedChanged += (s, e) =>
            {
                if (selectRdo.Checked)
                {
                    selectTxt.Enabled = true;
                    selectBtn.Enabled = true;

                    createBtn.Enabled = false;
                    createTxt.Enabled = false;
                }
                else
                {
                    selectBtn.Enabled = false;
                    selectTxt.Enabled = false;

                    createBtn.Enabled = true;
                    createTxt.Enabled = true;
                }
            };

            if (string.IsNullOrWhiteSpace(model.EventHandlerName))
            {
                selectRdo.Checked = true;
            }


            dialog.DataContext = model;
            handlerTxt.TextBinding.BindDataContext((IToolboxEventModel m) => model.EventHandlerName);
            selectTxt.TextBinding.BindDataContext((IToolboxEventModel m) => model.SourceFile);

            selectBtn.Click += (s, e) =>
            {
                var openDialog = new OpenFileDialog
                {
                    Title = "Open Class",
                    Directory = new Uri("D:\\Demo")
                };

                openDialog.Filters.Add(new FileDialogFilter("C#", ".cs"));
                var result = openDialog.ShowDialog(Application.Instance.MainForm);

                if (result == DialogResult.Ok)
                {
                    model.SourceFile = openDialog.FileName;
                }
            };

            ok.Click += (s, e) =>
            {
                var fs = new FileSystem();
                var editor = new XenCodeEditor(fs, model.SourceFile);
                editor.PrependEventHandler(model.EventHandlerName, "object sender, EventArgs args");
                VisualStudioBridge.OpenFile(VisualStudioVersion.VS2015, model.SourceFile);

                dialog.Close();
            };
           
            return dialog;
        }
    }
}
