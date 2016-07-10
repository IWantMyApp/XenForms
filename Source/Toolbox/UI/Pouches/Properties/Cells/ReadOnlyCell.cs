using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells
{
    public class ReadOnlyCell : PropertyEditorCell<string>
    {
        public override Control OnCreate(CellEventArgs args)
        {
            var item = (IPropertyEditorModel) args.Item;

            if (item == null)
            {
                return new Label { Text = "Unsupported" };
            }

            var text = ItemBinding.GetValue(item);

            var label = new Label
            {
                Text = text
            };

            return new Panel
            {
                Padding = new Padding(2),
                Content = label
            };
        }


        public override void OnPaint(CellPaintEventArgs args) {}
    }
}