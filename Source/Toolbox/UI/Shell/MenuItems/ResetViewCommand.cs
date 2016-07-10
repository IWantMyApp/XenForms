using System;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class ResetViewCommand : Command
    {
        public ResetViewCommand()
        {
            MenuText = "Reset Window Layout";
        }

        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);
            ToolboxApp.Bus.Notify(new ResetWindowLayout());
        }
    }
}