using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Toolbox.UI.Pouches.Events.Cells
{
    public class ButtonCell : CustomCell
    {
        protected override Control OnCreateCell(CellEventArgs args)
        {
            var item = args.Item as IToolboxEventModel;
            var row = args.Row;

            var btn =  new Button
            {
                Width = 25,
                Height = 20,
                Text = "...",
                Enabled = false,
            };

            btn.Click += (s, e) =>
            {
                var dialog = EditEventDialog.Create(item);
                dialog.ShowModal(Application.Instance.MainForm);
                ToolboxApp.Bus.Notify(new RefreshWidgetEvent(row));
            };

            return btn;
        }
    }
}
