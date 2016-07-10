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
    public class CreateStackLayoutCommand : VisualTreeCommand
    {
        public CreateStackLayoutCommand(IMessageBus messaging, ToolboxSocket socket, ToolboxLogging log)
            : base(messaging, socket, log)
        {
            MenuText = "StackLayout";
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

            var grid = new TableLayout(2, 2)
            {
                Spacing = new Size(5, 10)
            };

            var footer = new TableLayout(3, 1)
            {
                Padding = new Padding(0, 10, 0, 0),
                Spacing = new Size(5, 0)
            };

            var spacing = new NumericUpDown
            {
                DecimalPlaces = 1,
                MinValue = 0
            };

            var orientation = new DropDown();

            var ok = new Button {Text = "Ok"};
            var cancel = new Button {Text = "Cancel"};

            footer.Add(null, 0, 0, true, false);
            footer.Add(ok, 1, 0, false, false);
            footer.Add(cancel, 2, 0, false, false);

            orientation.Items.Add("Vertical");
            orientation.Items.Add("Horizontal");
            orientation.SelectedIndex = 0;

            grid.Add("Orientation", 0, 0, true, true);
            grid.Add(orientation, 1, 0, false, true);
            grid.Add("Spacing", 0, 1, true, true);
            grid.Add(spacing, 1, 1, true, true);

            container.Add(grid, 0, 0, true, true);
            container.Add(footer, 0, 1, false, false);

            var dlg = new ConnectedDialog
            {
                Title = "Create StackLayout",
                Width = 300,
                Height = 155,
                Padding = AppStyles.WindowPadding,
                Content = container,
                AbortButton = cancel,
                DefaultButton = ok
            };

            cancel.Click += (sender, args) => dlg.Close();

            ok.Click += (sender, args) =>
            {
                var request = XenMessage.Create<CreateStackLayoutRequest>();
                request.ParentId = Node.Widget.Id;
                request.Orientation = orientation.SelectedValue.ToString();
                request.Spacing = spacing.Value;
                Socket.Send(request);

                Log.Info("Creating widget: StackLayout");
                dlg.Close();
            };

            dlg.ShowModal(Application.Instance.MainForm);
        }
    }
}