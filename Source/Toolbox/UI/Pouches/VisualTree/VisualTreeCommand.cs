using System.Linq;
using Eto.Forms;
using XenForms.Core.Networking;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Toolbox.Shell;
using XenForms.Toolbox.UI.Resources;

namespace XenForms.Toolbox.UI.Pouches.VisualTree
{
    public abstract class VisualTreeCommand : Command
    {
        protected ToolboxLogging Log;
        protected readonly IMessageBus Messaging;
        protected readonly ToolboxSocket Socket;
        protected VisualTreeNode Node;


        protected VisualTreeCommand(IMessageBus messaging, ToolboxSocket socket, ToolboxLogging log)
        {
            Log = log;
            Messaging = messaging;
            Socket = socket;

            messaging.Listen<VisualTreeNodeSelected>(payload =>
            {
                Node = payload.Node;
            });
        }


        public override MenuItem CreateMenuItem()
        {
            var menu = base.CreateMenuItem();

            menu.Validate += (s, e) =>
            {
                menu.Enabled = Node != null && Socket.IsConnected;
            };

            return menu;
        }


        protected DialogResult CheckOverwrite()
        {
            var dlgResult = DialogResult.Yes;

            if (Node.Widget.Children.Any() && !Node.Widget.AllowsManyChildren)
            {
                dlgResult = MessageBox.Show(Application.Instance.MainForm,
                    "Adding a control here will remove the existing children. Do you want to continue?",
                    AppResource.Title, MessageBoxButtons.YesNo, MessageBoxType.Question);
            }

            return dlgResult;
        }
    }
}