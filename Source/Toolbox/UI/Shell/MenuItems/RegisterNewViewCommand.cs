using System;
using Eto.Forms;
using Ninject;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class RegisterNewViewCommand : Command
    {
        public RegisterNewViewCommand()
        {
            MenuText = "Register New View";
            Image = AppImages.AddView;
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
            ToolboxApp.Bus.Notify(new ShowRegisterNewViewDialog());
        }
    }
}
