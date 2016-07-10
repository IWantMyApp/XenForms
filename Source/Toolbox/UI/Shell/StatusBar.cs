using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Toolbox.UI.Shell.Images;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Shell
{
    public class StatusBar : XenVisualElement
    {
        private TableLayout _layout;
        private Label _label;
        private Panel _feedback;


        public StatusBar()
        {
            CreateLayout();

            ToolboxApp.Bus.Listen<ShowStatusMessage>(args =>
            {
                Application.Instance.Invoke(() =>
                {
                    OnShowStatusMessage(args);
                });
            });
        }


        protected override Control OnDefineLayout()
        {
            return _layout;
        }


        protected override void OnUnload()
        {
            base.OnUnload();
            ToolboxApp.Bus.StopListening<ShowStatusMessage>();
        }


        private void OnShowStatusMessage(ShowStatusMessage args)
        {
            if (args.Message == null) return;
            var period = args.Message.EndsWith(".") ? string.Empty : ".";
            _label.Text = args.Message + period;
        }


        private void CreateLayout()
        {
            _label = new Label();

            _feedback = new Panel
            {
                Width = AppStyles.IconWidth,
                Height = AppStyles.IconHeight,
                //Content = AppImages.Settings
            };

            _layout = new TableLayout(3, 1)
            {
                Padding = new Padding(0, 5, 5, 0)
            };

            _layout.Add(_label, 0, 0, false, false);
            _layout.Add(null, 1, 0, true, false);
            _layout.Add(_feedback, 2, 0, false, false);
        }
    }
}