using System.Linq;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Widgets;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells.String
{
    public class EnumCellBuilder : BaseCellBuilder<string>
    {
        public override Control CreateLayout(PropertyEditorModel<string> model)
        {
            var xt = model.Property.XenType;

            if (xt.Descriptor.HasFlag(XenPropertyDescriptors.Flags))
            {
                return CreateFlagLayout(model);
            }

            return CreateStaticListLayout(model);
        }


        private Control CreateStaticListLayout(PropertyEditorModel<string> model)
        {
            var ddl = new DropDown();

            if (!model.Property.CanWrite)
            {
                ddl.Enabled = false;
            }

            foreach (var value in model.Property.XenType.PossibleValues)
            {
                ddl.Items.Add(value, value);
            }

            if (model.Property.Value == null)
            {
                var pv1 = model.Property.XenType.PossibleValues?.FirstOrDefault();
                model.ToolboxValue = pv1;
            }
            
            ddl.SelectedKeyBinding.BindDataContext((PropertyEditorModel<string> t) => (string)t.ToolboxValue);

            return ddl;
        }


        private Control CreateFlagLayout(PropertyEditorModel<string> model)
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
            var dlg = FlagEditDialog.Create(model);
            dlg.ShowModal(Application.Instance.MainForm);
        }
    }
}