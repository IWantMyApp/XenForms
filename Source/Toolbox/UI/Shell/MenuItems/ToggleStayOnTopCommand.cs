using System;
using Eto.Forms;
using XenForms.Toolbox.UI.Resources;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class ToggleStayOnTopCommand : CheckCommand 
    {
        public ToggleStayOnTopCommand()
        {
            MenuText = ToggleResource.Stay_on_top;
            ToolBarText = null;
            Image = AppImages.Pin;
            ToolTip = ToggleResource.Stay_on_top;
        }


        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);
            Application.Instance.MainForm.Topmost = Checked;
        }
    }
}
