using System;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Resources;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class DisconnectCommand : Command
    {
        public DisconnectCommand()
        {
            MenuText = CommonResource.Disconnect;
            ToolTip = ConnectResource.Disconnect_tooltip;
            Shortcut = Application.Instance.CommonModifier | Keys.D;
            Image = AppImages.Disconnect;
        }


        public override MenuItem CreateMenuItem()
        {
            var item = base.CreateMenuItem();

            item.Validate += (sender, args) =>
            {
                item.Enabled = ToolboxApp.SocketManager.IsConnected;
            };

            return item;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            var disconnect = ShowPrompt();

            if (disconnect == DialogResult.Yes)
            {
                ToolboxApp.SocketManager.Disconnect();
            }
        }


        public static DialogResult ShowPrompt()
        {
            return MessageBox.Show(Application.Instance.MainForm, ConnectResource.Are_you_sure_disconnect,
                AppResource.Title, MessageBoxButtons.YesNo, MessageBoxType.Question);
        }
    }
}