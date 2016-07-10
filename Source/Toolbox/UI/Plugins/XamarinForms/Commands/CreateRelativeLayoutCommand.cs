using Eto.Forms;
using XenForms.Core.Networking;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.Shell;
using XenForms.Toolbox.UI.Pouches.VisualTree;

namespace XenForms.Toolbox.UI.Plugins.XamarinForms.Commands
{
    [MenuPlacement("{80f4ca8b-d04b-48c4-89c1-f6c184fa3b00}", MenuLocation.VisualTree, "New", "Layout")]
    public class CreateRelativeLayoutCommand : VisualTreeCommand
    {
        public CreateRelativeLayoutCommand(IMessageBus messaging, ToolboxSocket socket, ToolboxLogging log)
            : base(messaging, socket, log)
        {
            MenuText = "RelativeLayout";
            ToolTip = "Disabled during Beta 1.";
        }


        public override MenuItem CreateMenuItem()
        {
            var menu = base.CreateMenuItem();

            menu.Validate += (s, e) =>
            {
                menu.Enabled = false;
            };

            return menu;
        }
    }
}
