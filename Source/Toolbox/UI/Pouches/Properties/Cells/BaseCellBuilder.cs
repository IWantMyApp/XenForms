using Eto.Forms;
using XenForms.Core.Toolbox;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells
{
    public abstract class BaseCellBuilder<T>
    {
        public abstract Control CreateLayout(PropertyEditorModel<T> model);
    }
}