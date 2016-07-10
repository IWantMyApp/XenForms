using System;
using Eto.Forms;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class ReleaseNotesCommand : Command
    {
        public ReleaseNotesCommand()
        {
            ToolTip = "Release Notes";
            MenuText = ToolTip;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);
            Application.Instance.Open("https://www.xenforms.com/release-notes/");
        }
    }
}