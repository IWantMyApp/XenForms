using System;
using Eto.Forms;
using Ninject;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Pouches.Properties;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class RegisterNewTypeCommand : Command
    {
        public RegisterNewTypeCommand()
        {
            MenuText = "Register New Property Type";
            Image = AppImages.Plus;
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

            var dlg = ToolboxApp.Services.Get<RegisterNewTypeDialog>();
            dlg.Show();
        }
    }
}