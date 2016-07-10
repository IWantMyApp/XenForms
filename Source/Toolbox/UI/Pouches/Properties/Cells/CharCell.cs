using Eto.Forms;
using XenForms.Core.Toolbox;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells
{
    [PropertyEditor(typeof(char))]
    public class CharCell : PropertyEditorCell<char>
    {
        public override Control OnCreate(CellEventArgs args)
        {
            Initialize(args);

            var ctrl = new TextBox {MaxLength = 1};

            if (!Model.Property.CanWrite)
            {
                ctrl.ReadOnly = true;
            }

            ctrl.TextBinding.BindDataContext((PropertyEditorModel<string> t) => t.ToolboxValue.ToString());

            var layout = new TableLayout(1, 1);
            layout.Add(ctrl, 0, 0, true, true);

            return layout;
        }


        public override void OnPaint(CellPaintEventArgs args) { }
    }
}
