using Eto.Forms;
using XenForms.Core.Toolbox;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells
{
    public abstract class PropertyEditorCell<T> : PropertyCellType<T>, IPropertyEditor<T>
    {
        public override string Identifier => GetType().AssemblyQualifiedName;
        public PropertyEditorModel<T> Model { get; set; }


        protected void Initialize(CellEventArgs args)
        {
            var model = args.Item as PropertyEditorModel<T>;

            if (model != null)
            {
                Model = model;
            }
        }
    }
}