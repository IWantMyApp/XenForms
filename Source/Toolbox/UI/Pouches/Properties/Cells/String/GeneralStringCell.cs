using Eto.Forms;
using XenForms.Core.Widgets;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells.String
{
    public class GeneralStringCell : PropertyEditorCell<string>
    {
        public override Control OnCreate(CellEventArgs args)
        {
            BaseCellBuilder<string> builder;
            Initialize(args);

            var desc = Model.Property?.XenType?.Descriptor ?? XenPropertyDescriptors.None;

            switch (desc)
            {
                case XenPropertyDescriptors.Literals | XenPropertyDescriptors.Flags:
                case XenPropertyDescriptors.Literals:
                    builder = new EnumCellBuilder();
                    break;
                case XenPropertyDescriptors.Image:
                    builder = new ImageCellBuilder();
                    break;
                default:
                    builder = new TextCellBuilder();
                    break;
            }

            return builder.CreateLayout(Model);
        }


        public override void OnPaint(CellPaintEventArgs args) {}
    }
}