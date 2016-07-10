using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.Project;
using XenForms.Toolbox.UI.Shell;
using XenForms.Toolbox.UI.Shell.Images;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Pouches.VisualTree
{
    public class RegisterNewViewDialog
    {
        private readonly RegisterNewViewModel _vm;
        private ConnectedDialog _dlg;
        private readonly List<ButtonMenuItem> _targets;


        public RegisterNewViewDialog(RegisterNewViewModel vm)
        {
            _vm = vm;
            _targets = new List<ButtonMenuItem>();
            _vm.RecreateMenuItems += OnRecreateMenuItems;
        }


        public void Register(IEnumerable<MenuItem> items)
        {
            try
            {
                var newItem = items?.FirstOrDefault(i => i.Text == "New") as ButtonMenuItem;
                if (newItem == null) return;

                var userDefined = new ButtonMenuItem
                {
                    Text = "User Views"
                };

                newItem.Items.Add(new SeparatorMenuItem());
                newItem.Items.Add(userDefined);

                _targets.Add(userDefined);
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, "Unable to configure user view menus. User will not be able to add custom views.");
            }
        }


        public void Show()
        {
            _vm.Show();

            var desc = new Label
            {
                Text = "Once a view has been added, it will be available for use via the visual tree's context menu. Please enter the type's full name.\n\ne.g. MyProject.Views.ImageButton"
            };

            var textBox = new TextBox();
            var grid = new GridView
            {
                ShowHeader = true,
                GridLines = GridLines.Horizontal
            };

            grid.Columns.Add(new GridColumn { AutoSize = true, HeaderText = "Type Name", DataCell = new TextBoxCell("TypeName"), Editable = false});
            grid.DataStore = _vm.Views;

            var container = new TableLayout(1, 4) { Spacing = new Size(0, 10) };
            var textrow = new TableLayout(2, 1) {Spacing = new Size(5, 0) };
            var footer = new TableLayout(4, 1) { Spacing = new Size(5, 0) };

            var add = new Button {Text = "Add"};
            var delete = new Button { Text = "Delete" };
            var ok = new Button { Text = "Ok" };
            var cancel = new Button { Text = "Cancel" };

            footer.Add(delete, 0, 0, false, false);
            footer.Add(null, 1, 0, true, false);
            footer.Add(cancel, 2, 0, false, false);
            footer.Add(ok, 3, 0, false, false);

            container.Add(desc, 0, 0, true, false);
            container.Add(textrow, 0, 1, false, false);
            container.Add(grid, 0, 2, false, true);
            container.Add(footer, 0, 3, false, false);

            textrow.Add(textBox, 0, 0, true, false);
            textrow.Add(add, 1, 0, false, false);

            _dlg = new ConnectedDialog
            {
                Padding = AppStyles.WindowPadding,
                Content = container,
                Width = 350,
                Height = 400,
                Icon = AppImages.Xf,
                Title = "Register New View",
                AbortButton = cancel,
                DataContext = _vm
            };

            textBox.TextBinding.BindDataContext((RegisterNewViewModel m) => m.TypeName);
            grid.SelectedItemBinding.BindDataContext((RegisterNewViewModel m) => m.SelectedItem);
            delete.Bind(d => d.Enabled, _vm, vm => vm.DeleteEnabled);
            add.Bind(d => d.Enabled, _vm, vm => vm.AddEnabled);

            add.Click += OnAdd;
            delete.Click += OnDelete;
            ok.Click += OnOk;
            textBox.KeyDown += OnKeyDown;
            cancel.Click += OnCancel;

            _dlg.ShowModal(Application.Instance.MainForm);
        }


        public void Initialize()
        {
            _vm.Initialize();
        }


        public void Close()
        {
            _dlg.Close();
        }


        private void AddMenuItems(ProjectView[] views)
        {
            try
            {
                foreach (var target in _targets)
                {
                    target.Items.Clear();

                    foreach (var pv in views)
                    {
                        var bmi = new ButtonMenuItem
                        {
                            Tag = pv,
                            Text = pv.TypeName
                        };

                        bmi.Validate += OnValidate;
                        bmi.Click += OnItemClicked;
                        target.Items.Add(bmi);
                    }
                }
            }
            catch (Exception oex) 
            {
                ToolboxApp.Log.Error(oex, "Unable to add user views to targets.");
            }
        }


        private void ClearTargets()
        {
            try
            {
                foreach (var target in _targets)
                {
                    target.Items.Clear();
                }
            }
            catch (Exception oex)
            {
                ToolboxApp.Log.Error(oex, "Unable to add user views to targets.");
            }
        }


        private void OnAdd(object sender, EventArgs e)
        {
            _vm.AddView();
        }


        private void OnValidate(object sender, EventArgs e)
        {
            try
            {
                var bmi = sender as ButtonMenuItem;
                var pv = bmi?.Tag as ProjectView;
                if (pv == null) return;
                bmi.Enabled = _vm.SelectedNode.Widget.CanAttach(pv.TypeName);
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, "An error occurred while sending the validating the create view menu item.");
            }
        }


        private void OnItemClicked(object sender, EventArgs e)
        {
            try
            {
                var bmi = sender as ButtonMenuItem;
                var pv = bmi?.Tag as ProjectView;
                if (pv == null) return;

                ToolboxApp.Designer.CreateWidget(_vm.SelectedNode.Widget.Id, pv.TypeName);
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, "An error occurred while sending the create widget command.");
            }
        }


        private void OnCancel(object sender, EventArgs e)
        {
            _vm.DiscardChanges();
            Close();
        }


        private void OnRecreateMenuItems(object sender, RecreateMenuItemsEventArgs e)
        {
            if (e.Views == null)
            {
                ClearTargets();
            }
            else
            {
                AddMenuItems(e.Views);
            }
        }


        private void OnDelete(object sender, EventArgs e)
        {
            var result = MessageBox.Show(_dlg,
                $"Are you sure you want to delete \"{_vm.SelectedItem.TypeName}\"? You will no longer be able to add views of this type through the toolbox.",
                "XenForms",
                MessageBoxButtons.YesNo,
                MessageBoxType.Question);

            if (result == DialogResult.Yes)
            {
                _vm.Delete();
            }
        }


        private void OnOk(object sender, EventArgs e)
        {
            _vm.Save();
            Close();
        }


        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Keys.Enter)
            {
                _vm.AddView();
            }
        }
    }
}