using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Resources;
using XenForms.Toolbox.UI.Shell;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells.String
{
    public static class TextEditDialog
    {
        public static Dialog Create(PropertyEditorModel<string> model)
        {
            var value = model.ToolboxValue as string;

            var dlg = new ConnectedDialog
            {
                Title = "Edit String",
                Padding = AppStyles.WindowPadding,
                Width = 500,
                Height = 400,
            };

            var footer = new TableLayout(2, 1);
            var layout = new TableLayout(1, 2)
            {
                Spacing = new Size(0, 10)
            };

            var text = new TextArea
            {
                Text = value,
                Wrap = true,
                DataContext = model
            };

            text.TextBinding.BindDataContext((PropertyEditorModel<string> m) => (string) m.ToolboxValue);

            text.ReadOnly = !model.Property.CanWrite;

            var ok = new Button { Text = CommonResource.Ok };
            ok.Click += (s, e) => dlg.Close();

            layout.Add(text, 0, 0, true, true);
            footer.Add(null, 0, 0, true, false);
            footer.Add(ok, 1, 0, false, false);
            layout.Add(footer, 0, 1, true, false);

            dlg.Content = layout;
            dlg.AbortButton = ok;

            return dlg;
        }
    }
}
