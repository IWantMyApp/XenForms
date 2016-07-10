using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Widgets;
using XenForms.Toolbox.UI.Pouches.Properties.Cells.Collections;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells.Object
{
    public class ObjectCell : PropertyEditorCell<object>
    {
        public override Control OnCreate(CellEventArgs args)
        {
            Initialize(args);

            var container = new TableLayout(2, 1)
            {
                Spacing = AppStyles.PropertyCellSpacing
            };

            var lbl = new Label
            {
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Height = AppStyles.PropertyCellHeight,
                Text = "Complex Object"
            };

            var btn = new Button
            {
                Width = AppStyles.IconWidth,
                Text = "..."
            };

            // this captures the model. why??
            var model = Model;
            btn.Click += (s, e) => ShowEditDialog(model);

            container.Add(lbl, 0, 0, true, false);
            container.Add(btn, 1, 0, false, false);

            return container;
        }


        public void ShowEditDialog(PropertyEditorModel<object> model)
        {
            var treeNode = model?.VisualNode;
            var xt = model?.Property?.XenType;

            if (treeNode == null)
            {
                ToolboxApp.Log.Error($"The visual node for {model?.DisplayName} was null.");
                return;
            }

            if (xt == null)
            {
                ToolboxApp.Log.Error($"The TypeEditor was null. Could not open custom object edit dialog for {model?.DisplayName}.");
                return;
            }

            Dialog dlg;

            if (xt.Descriptor.HasFlag(XenPropertyDescriptors.Enumerable))
            {
                dlg = CollectionEditDialog.Create(model, treeNode);
            }
            else
            {
                dlg = ObjectEditDialog.Create(model, treeNode);
            }

            dlg.ShowModal(Application.Instance.MainForm);
        }


        public override void OnPaint(CellPaintEventArgs args) {}
    }
}