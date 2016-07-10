using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells.String
{
    public class TextCellBuilder : BaseCellBuilder<string>
    {
        public override Control CreateLayout(PropertyEditorModel<string> model)
        {
            var layout = new TableLayout(2, 1)
            {
                Spacing = AppStyles.PropertyCellSpacing
            };

            var txt = new TextBox
            {
                Height = AppStyles.PropertyCellHeight,
            };

            if (!model.Property.CanWrite)
            {
                txt.ReadOnly = true;
            }

            txt.TextBinding.BindDataContext((PropertyEditorModel<string> t) => (string) t.ToolboxValue);

            var btn = new Button
            {
                Width = AppStyles.IconWidth,
                Text = "..."
            };

            btn.Click += (s, e) => OpenDialog(model);

            layout.Add(txt, 0, 0, true, false);
            layout.Add(btn, 1, 0, false, false);

            return layout;
        }


        private void OpenDialog(PropertyEditorModel<string> model)
        {
            var dlg = TextEditDialog.Create(model);
            dlg.ShowModal(Application.Instance.MainForm);
        }
    }
}
