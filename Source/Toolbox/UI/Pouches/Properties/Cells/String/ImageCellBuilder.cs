using System;
using System.IO;
using System.Linq;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells.String
{
    public class ImageCellBuilder : BaseCellBuilder<string>
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
                ReadOnly = true
            };

            if (!model.Property.CanWrite)
            {
                txt.ReadOnly = true;
            }

            txt.TextBinding.BindDataContext((PropertyEditorModel<string> t) => (string) t.DisplayValue);

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
            var dlg = new OpenFileDialog();

            dlg.Filters.Add(new FileDialogFilter("JPG", "jpg", "jpeg"));
            dlg.Filters.Add(new FileDialogFilter("PNG", "png"));

            var result = dlg.ShowDialog(Application.Instance.MainForm);

            if (result == DialogResult.Ok)
            {
                var filename = dlg.Filenames.ElementAt(0);
                var data = File.ReadAllBytes(filename);
                string b64;

                using (var ms = new MemoryStream(data))
                {
                    b64 = Convert.ToBase64String(ms.ToArray());
                }

                // todo: fixme here. this thinks it needs to be set twice.
                model.Property.Metadata = filename;
                model.ToolboxValue = " ";
                model.SaveInBase64 = true;
                model.ToolboxValue = b64;
                model.DisplayValue = filename;
            }
        }
    }
}