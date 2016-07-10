using System;
using Eto.Forms;
using XenForms.Toolbox.UI.Resources;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class QuitCommand : Command
    {
        public QuitCommand()
        {
            MenuText = CommonResource.Quit;
            Shortcut = Application.Instance.CommonModifier | Keys.Q;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);
            Application.Instance.Quit();
        }


        public static DialogResult ShowPrompt()
        {
            return MessageBox.Show(Application.Instance.MainForm, AppResource.Are_you_sure_you_want_to_quit,
                AppResource.Title, MessageBoxButtons.YesNo, MessageBoxType.Question);
        }
    }
}