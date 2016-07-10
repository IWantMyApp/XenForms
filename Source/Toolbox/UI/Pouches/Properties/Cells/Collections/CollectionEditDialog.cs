using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Messages;
using XenForms.Core.Reflection;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Widgets;
using XenForms.Toolbox.UI.Pouches.Properties.Cells.Object;
using XenForms.Toolbox.UI.Resources;
using XenForms.Toolbox.UI.Shell;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells.Collections
{
    public static class CollectionEditDialog
    {
        public static Dialog Create(PropertyEditorModel<object> model, VisualTreeNode treeNode)
        {
            var widgetId = treeNode.Widget.Id;
            var propertyName = model.Property.PropertyName;
            var pv = model.Property.XenType.PossibleValues;

            var grid = new PropertyEditorGridView();

            var dlg = new ConnectedDialog
            {
                Title = $"Edit {model.DisplayName}",
                Padding = AppStyles.WindowPadding,
                Width = 650,
                Height = 475
            };

            var collection = new ListBox();

            var footer = new TableLayout(4, 1)
            {
                Padding = new Padding(0, 10, 0, 0),
                Spacing = new Size(5, 5)
            };

            var ok = new Button { Text = "Ok" };
            var add = new Button {Text = "Add"};
            var del = new Button {Text = "Delete"};

            footer.Add(del, 0, 0, false, false);
            footer.Add(add, 1, 0, false, false);
            footer.Add(null, 2, 0, true, false);
            footer.Add(ok, 3, 0, false, false);

            var splitter = new Splitter
            {
                SplitterWidth = 5,
                FixedPanel = SplitterFixedPanel.Panel1,
                Panel1 = collection,
                Panel2 = grid,
                RelativePosition = dlg.Width * .35
            };

            var container = new TableLayout(1, 2);
            container.Add(splitter, 0, 0, true, true);
            container.Add(footer, 0, 1, true, false);

            dlg.Content = container;
            dlg.AbortButton = ok;
            dlg.DefaultButton = ok;

            ok.Click += (s, e) => { dlg.Close(); };

            dlg.Shown += (sender, args) =>
            {
                ToolboxApp.Log.Trace($"Showing {nameof(ObjectEditDialog)} for {treeNode.DisplayName}.");
            };

            dlg.Closing += (sender, args) =>
            {
                ToolboxApp.Bus.StopListening<CollectionPropertiesReceived>();
                ToolboxApp.Bus.StopListening<EditCollectionResponseReceived>();
                ToolboxApp.Bus.StopListening<ReplacedWidgetCollection>();

                ToolboxApp.Log.Trace($"Closing {nameof(ObjectEditDialog)} for {treeNode.DisplayName}.");
            };

            del.Click += (sender, args) =>
            {
                var item = collection.SelectedValue as ListItem;
                var path = (string) item?.Tag;
                if (string.IsNullOrWhiteSpace(path)) return;

                var res = MessageBox.Show(Application.Instance.MainForm,
                    $"Are you sure you want to remove {path}?",
                    AppResource.Title, MessageBoxButtons.YesNo, MessageBoxType.Question);

                if (res == DialogResult.Yes)
                {
                    ToolboxApp
                        .Designer
                        .EditCollection(treeNode.Widget.Id, EditCollectionType.Delete, path);
                }
            };

            add.Click += (sender, args) =>
            {
                ToolboxApp
                    .AppEvents
                    .Designer
                    .EditCollection(treeNode.Widget.Id, EditCollectionType.Add, model.Property.PropertyName);
            };

            collection.SelectedIndexChanged += (sender, args) =>
            {
                grid.Clear();
                var item = collection.SelectedValue as ListItem;

                if (item != null)
                {
                    var path = (string) item.Tag;
                    ToolboxApp.Designer.GetObjectProperties(widgetId, path);
                }
            };

            ToolboxApp.AppEvents.Bus.Listen<CollectionPropertiesReceived>(args =>
            {
                var index = ReflectionMethods.GetIndexerValue(args.WidgetName);
                if (index == null) return;

                Application.Instance.Invoke(() =>
                {
                    grid.ShowEditors(treeNode, args.Properties);
                });
            });

            ToolboxApp.AppEvents.Bus.Listen<ReplacedWidgetCollection>(args =>
            {
                Application.Instance.Invoke(() =>
                {
                    var length = GetCollectionCount(args.Widget, propertyName);
                    CreateListItems(collection, propertyName, length);
                });
            });

            ToolboxApp.AppEvents.Bus.Listen<EditCollectionResponseReceived>(args =>
            {
                Application.Instance.Invoke(() =>
                {
                    if (!args.Successful)
                    {
                        MessageBox.Show(Application.Instance.MainForm,
                            $"There was an error performing the '{args.Type}' operation. Check the log for more information.",
                            AppResource.Title, MessageBoxButtons.OK, MessageBoxType.Error);

                        ToolboxApp.Log.Error(args.Message);
                    }
                });
            });

            // populate list box
            if (pv != null)
            {
                int length;
                var parsed = int.TryParse(pv[0], out length);

                if (parsed)
                {
                    CreateListItems(collection, propertyName, length);
                }
            }

            return dlg;
        }


        private static int GetCollectionCount(XenWidget widget, string propertyName)
        {
            var result = 0;
            var property = widget.Properties.FirstOrDefault(p => p.PropertyName == propertyName);
            var pv = property?.XenType.PossibleValues?[0];
            if (pv == null) return result;

            int.TryParse(pv, out result);
            return result;
        }


        private static void CreateListItems(ListBox collection, string propertyName, int length)
        {
            collection.Items.Clear();

            for (var i = 0; i < length; i++)
            {
                var path = $"{propertyName}[{i}]";

                var item = new ListItem
                {
                    Text = path,
                    Key = i.ToString(),
                    Tag = path
                };

                collection.Items.Add(item);
            }
        }
    }
}
