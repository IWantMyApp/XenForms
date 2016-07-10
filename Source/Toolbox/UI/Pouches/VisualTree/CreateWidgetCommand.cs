using System;
using Eto.Forms;
using XenForms.Core.Networking;
using XenForms.Core.Plugin;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.Shell;

namespace XenForms.Toolbox.UI.Pouches.VisualTree
{
    public class CreateWidgetCommand : VisualTreeCommand
    {
        private ViewRegistration Registration { get; }


        public CreateWidgetCommand(IMessageBus messaging, ToolboxSocket socket, ToolboxLogging log, ViewRegistration registration) 
            : base(messaging, socket, log)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));
            
            if (string.IsNullOrWhiteSpace(registration.Type?.FullName))
            {
                throw new InvalidOperationException("The view type was not specified.");
            }

            Registration = registration;
            MenuText = registration.Type.ShortName;
        }


        public override MenuItem CreateMenuItem()
        {
            var menu = base.CreateMenuItem();

            menu.Validate += (s, e) =>
            {
                menu.Enabled = Node.Widget.CanAttach(Registration);
            };

            return menu;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            var overwrite = CheckOverwrite();

            if (overwrite == DialogResult.Yes)
            {
                var tn = Registration.Type.FullName;
                ToolboxApp.Designer.CreateWidget(Node.Widget.Id, tn);
                ToolboxApp.Log.Info($"Creating widget: {tn}.");
            }
        }
    }
}