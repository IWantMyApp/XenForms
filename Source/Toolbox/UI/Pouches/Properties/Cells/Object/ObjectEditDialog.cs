using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Widgets;
using XenForms.Toolbox.UI.Shell;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells.Object
{
    public static class ObjectEditDialog
    {
        public static Dialog Create(PropertyEditorModel<object> model, VisualTreeNode treeNode)
        {
            var id = treeNode.Widget.Id;
            var editable = false;

            var dlg = new ConnectedDialog
            {
                Title = $"Edit {model.DisplayName}",
                Padding = AppStyles.WindowPadding,
                Width = 365,
                Height = 420
            };

            var grid = new PropertyEditorGridView();

            var constructorDdl = new DropDown
            {
                Enabled = false
            };

            // default to show the user, so it doesn't flicker into view.
            constructorDdl.Items.Add("Not Applicable", "Not Applicable");
            constructorDdl.SelectedIndex = 0;

            var createBtn = new Button
            {
                Enabled = false,
                Text = "Create"
            };

            var showBtn = new Button
            {
                Enabled = false,
                Text = "Show"
            };

            ToolboxApp.AppEvents.Bus.Listen<ConstructorCalled>(args =>
            {
                Application.Instance.Invoke(() =>
                {
                    dlg.Close();
                });
            });

            ToolboxApp.AppEvents.Bus.Listen<ConstructorsReceived>(args =>
            {
                Application.Instance.Invoke(() =>
                {
                    showBtn.Enabled = editable;
                    constructorDdl.Enabled = editable;

                    if (editable)
                    {
                        // clear default
                        constructorDdl.Items.Clear();

                        foreach (var ctor in args.Type.Constructors)
                        {
                            var item = new ListItem
                            {
                                Text = ctor.DisplayName,
                                Tag = ctor
                            };

                            constructorDdl.Items.Add(item);
                        }
                    }

                    if (constructorDdl.Items.Count > 0)
                    {
                        constructorDdl.SelectedIndex = 0;
                    }
                });
            });

            ToolboxApp.AppEvents.Bus.Listen<ObjectPropertiesReceived>(args =>
            {
                Application.Instance.Invoke(() =>
                {
                    var xt = model.Property.XenType;
                    editable = xt.Descriptor.HasFlag(XenPropertyDescriptors.ValueType) && model.Property.CanWrite;

                    grid.ShowEditors(treeNode, args.Properties, editable);
                });
            });

            ToolboxApp.Designer.GetObjectProperties(id, model.Property.Path);
            ToolboxApp.Designer.GetConstructors(model.FullTypeName);

            var propLabel = new Label
            {
                Text = model.Property.CanWrite ? "Properties" : "Properties are readonly"
            };

            var container = new TableLayout(1, 7)
            {
                Spacing = new Size(10, 5)
            };

            var constructors = new TableLayout(2,1)
            {
                Spacing = new Size(5, 5)
            };

            constructors.Add(constructorDdl, 0, 0, true, false);
            constructors.Add(showBtn, 1, 0, false, false);

            var predefinedDdl = new DropDown();

            var footer = new TableLayout(3, 1)
            {
                Padding = new Padding(0, 5, 0, 0),
                Spacing = new Size(5, 5),
            };

            var escapeBtn = new Button {Text = "Ok"};

            footer.Add(null, 0, 0, true, false);
            footer.Add(createBtn, 1, 0, false, false);
            footer.Add(escapeBtn, 2, 0, false, false);

            container.Add("Constructors", 0, 0, true, false);
            container.Add(constructors, 0, 1, true, false);
            container.Add($"{model.ShortTypeName} predefined values", 0, 2, true, false);
            container.Add(predefinedDdl, 0, 3, true, false);
            container.Add(propLabel, 0, 4, true, false);
            container.Add(grid, 0, 5, true, true);
            container.Add(footer, 0, 6, true, false);

            dlg.Content = container;

            escapeBtn.Click += (s, e) => { dlg.Close(); };

            dlg.AbortButton = escapeBtn;
            dlg.DefaultButton = escapeBtn;

            if (!model.Property.CanWrite)
            {
                predefinedDdl.Items.Add("Not Applicable", "Disabled");
                predefinedDdl.SelectedIndex = 0;
                predefinedDdl.Enabled = false;
            }
            else
            {
                // refactor
                predefinedDdl.Items.Add("Custom", "Custom");

                var vals = model.Property?.XenType?.PossibleValues;
                if (vals != null)
                {
                    foreach (var v in vals)
                    {
                        predefinedDdl.Items.Add(v, v);
                    }
                }

                predefinedDdl.SelectedIndex = 0;
            }

            predefinedDdl.SelectedIndexChanged += (sender, args) =>
            {
                grid.Enabled = predefinedDdl.SelectedIndex == 0;

                if (predefinedDdl.SelectedIndex != 0)
                {
                    model.ToolboxValue = predefinedDdl.SelectedValue.ToString();
                }
            };

            dlg.Shown += (sender, args) =>
            {
                ToolboxApp.Log.Trace($"Showing {nameof(ObjectEditDialog)} for {treeNode.DisplayName}.");
            };

            dlg.Closing += (sender, args) =>
            {
                ToolboxApp.Bus.StopListening<ObjectPropertiesReceived>();
                ToolboxApp.Bus.StopListening<ConstructorsReceived>();
                ToolboxApp.Bus.StopListening<ConstructorCalled>();

                ToolboxApp.Log.Trace($"Closing {nameof(ObjectEditDialog)} for {treeNode.DisplayName}.");
            };

            createBtn.Click += (sender, args) =>
            {
                var item = constructorDdl.SelectedValue as ListItem;
                var ctor = item?.Tag as XenConstructor;

                if (ctor != null)
                {
                    ToolboxApp.Designer.CallConstructor(treeNode.Widget.Id, model.Property, ctor);
                }
            };

            showBtn.Click += (sender, args) =>
            {
                var item = constructorDdl.SelectedValue as ListItem;
                var tag = item?.Tag as XenConstructor;

                if (tag != null)
                {
                    predefinedDdl.Enabled = false;
                    createBtn.Enabled = true;
                    escapeBtn.Text = "Cancel";

                    grid.ShowEditors(treeNode, tag);
                }
                else
                {
                    ToolboxApp.Log.Error($"The selected constructor for {treeNode.DisplayName} did not contain a valid tag.");
                }
            };

            return dlg;
        }
    }
}
