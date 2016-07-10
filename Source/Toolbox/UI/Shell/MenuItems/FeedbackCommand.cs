using System;
using Eto.Forms;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class FeedbackCommand : Command
    {
        public FeedbackCommand()
        {
            ToolTip = "User Feedback";
            MenuText = ToolTip;
            Image = AppImages.Comments;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);
            Application.Instance.Open("https://github.com/michaeled/XenForms/");
        }
    }
}