using Eto.Forms;
using XenForms.Core.Toolbox;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells
{
    [PropertyEditor(typeof(bool))]
    public class BoolCell : PropertyEditorCell<bool>
    {
        public override Control OnCreate(CellEventArgs args)
        {
            Initialize(args);

            var ctrl = new CheckBox { Text = string.Empty};

            if (!Model.Property.CanWrite)
            {
                ctrl.Enabled = false;
            }

            if (!Model.UseDefaultValue)
            {
                Model.ToolboxValue = false;
            }

            ctrl.CheckedBinding.BindDataContext((PropertyEditorModel<bool?> t) => (bool?) t.ToolboxValue);
            ctrl.TextBinding.BindDataContext((PropertyEditorModel<string> t) => (string) t.ToolboxValue);

            var layout = new TableLayout(1, 1);
            layout.Add(ctrl, 0, 0, true, true);

            return layout;
        }


        public override void OnPaint(CellPaintEventArgs args) { }
    }
}
