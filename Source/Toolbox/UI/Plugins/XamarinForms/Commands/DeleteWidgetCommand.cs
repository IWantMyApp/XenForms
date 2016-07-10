using System;
using Eto.Forms;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.Shell;
using XenForms.Toolbox.UI.Pouches.VisualTree;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Plugins.XamarinForms.Commands
{
    [MenuPlacement("80f4ca8b-d04b-48c4-89c1-f6c184fa3b00", MenuLocation.VisualTree)]
    public class DeleteWidgetCommand : VisualTreeCommand
    {
        public DeleteWidgetCommand(IMessageBus messaging, ToolboxSocket socket, ToolboxLogging log) 
            : base(messaging, socket, log)
        {
            MenuText = "Delete";
            Image = AppImages.Delete;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            if (!Node.Widget.CanDelete)
            {
                MessageBox.Show(Application.Instance.MainForm,
                    "This view cannot be removed.", "XenForms",
                    MessageBoxButtons.OK, MessageBoxType.Question);

                return;
            }

            var count = Node.Widget.Children?.Count;
            var warning = $"Are you sure you want to remove this {Node.DisplayName}?";

            if (count != null && count > 0)
            {
                warning += $"\nIt has {count} " + (count == 1 ? "child." : "children.");
            }

            var response = MessageBox.Show(Application.Instance.MainForm,
                warning, "XenForms",
                MessageBoxButtons.YesNo,
                MessageBoxType.Question);

            if (response == DialogResult.Yes)
            {
                var msg = XenMessage.Create<DeleteWidgetRequest>();
                msg.WidgetId = Node.Widget.Id;
                Socket.Send(msg);

                Log.Info($"Removing widget: {Node.DisplayName}");
            }
        }
    }
}