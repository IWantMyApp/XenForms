using System;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class NewPageCommand : Command
    {
        public NewPageCommand()
        {
            MenuText = "New Page";
            ToolTip = "Create New Page";
            Image = AppImages.NewFile;
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

            var result = MessageBox.Show(Application.Instance.MainForm,
                "Are you sure you want to create a new page? Save any unfinished work.",
                "XenForms",
                MessageBoxButtons.YesNo,
                MessageBoxType.Question);

            if (result == DialogResult.Yes)
            {
                ToolboxApp.Project.NewPage();
            }
        }
    }
}