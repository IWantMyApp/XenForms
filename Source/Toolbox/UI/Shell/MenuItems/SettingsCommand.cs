using System;
using Eto.Forms;
using Ninject;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Resources;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class SettingsCommand : Command
    {
        public SettingsCommand()
        {
            MenuText = $"{SettingsResource.Dialog_title}...";
            ToolBarText = null;
            Image = AppImages.Settings;
            ToolTip = SettingsResource.Dialog_title;
        }


        protected override async void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            var dialog = ToolboxApp.Services.Get<SettingsDialog>();
            ToolboxUI.Activate(dialog);

            await dialog.ShowAsync(Application.Instance.MainForm);
        }
    }
}
