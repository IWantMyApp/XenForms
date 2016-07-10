using System;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.Shell;
using XenForms.Toolbox.UI.Pouches.VisualTree;
using XenForms.Toolbox.UI.Shell;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Plugins.XamarinForms.Commands
{
    [MenuPlacement("{80f4ca8b-d04b-48c4-89c1-f6c184fa3b00}", MenuLocation.VisualTree, "New", "Layout")]
    public class CreateGridCommand : VisualTreeCommand
    {
        public CreateGridCommand(IMessageBus messaging, ToolboxSocket socket, ToolboxLogging log)
            : base(messaging, socket, log)
        {
            MenuText = "Grid";
        }

        public override MenuItem CreateMenuItem()
        {
            var menu = base.CreateMenuItem();

            menu.Validate += (s, e) =>
            {
                menu.Enabled = Node.Widget.CanAttach(Node.Widget);
            };

            return menu;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            var overwrite = CheckOverwrite();
            if (overwrite == DialogResult.No) return;

            var container = new TableLayout(1, 2)
            {
                Spacing = new Size(5, 5)
            };

            var grid = new TableLayout(4, 2)
            {
                Spacing = new Size(10, 10)
            };

            var footer = new TableLayout(3, 1)
            {
                Padding = new Padding(0, 10, 0, 0),
                Spacing = new Size(5, 0)
            };

            var columns = new NumericUpDown { Value = 1, DecimalPlaces = 0, MinValue = 0};
            var rows = new NumericUpDown { Value = 1, DecimalPlaces = 0, MinValue = 0 };
            var columnSpacing = new NumericUpDown { Value = 0, DecimalPlaces = 0, MinValue = 0 };
            var rowSpacing = new NumericUpDown { Value = 0, DecimalPlaces = 0, MinValue = 0 };

            var ok = new Button {Text = "Ok"};
            var cancel = new Button {Text = "Cancel"};

            footer.Add(null, 0, 0, true, false);
            footer.Add(ok, 1, 0, false, false);
            footer.Add(cancel, 2, 0, false, false);

            grid.Add("Columns", 0, 0, false, true);
            grid.Add(columns, 1, 0, false, true);
            grid.Add("Rows", 0, 1, false, true);
            grid.Add(rows, 1, 1, false, true);
            grid.Add("Column Spacing", 2, 0, false, true);
            grid.Add(columnSpacing, 3, 0, false, true);
            grid.Add("Row Spacing", 2, 1, false, true);
            grid.Add(rowSpacing, 3, 1, false, true);

            container.Add(grid, 0, 0, true, true);
            container.Add(footer, 0, 1, false, false);

            var dlg = new ConnectedDialog
            {
                Title = "Create Grid",
                Width = 365,
                Height = 154,
                Padding = AppStyles.WindowPadding,
                Content = container,
                AbortButton = cancel,
                DefaultButton = ok
            };

            cancel.Click += (sender, args) => dlg.Close();

            ok.Click += (sender, args) =>
            {
                var request = XenMessage.Create<CreateGridRequest>();
                request.ParentId = Node.Widget.Id;
                request.Columns = (int) columns.Value;
                request.ColumnSpacing = (int) columnSpacing.Value;
                request.Rows = (int) rows.Value;
                request.RowSpacing = (int) rowSpacing.Value;
                Socket.Send(request);
                
                Log.Info($"Creating widget: Grid[{request.Rows}, {request.Columns}].");
                dlg.Close();
            };

            dlg.ShowModal(Application.Instance.MainForm);
        }
    }
}