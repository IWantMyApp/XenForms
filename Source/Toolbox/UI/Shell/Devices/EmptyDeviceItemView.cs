using Eto.Drawing;
using Eto.Forms;

namespace XenForms.Toolbox.UI.Shell.Devices
{
    public class EmptyDeviceItemView : Panel
    {
        public string Description { get; }


        public EmptyDeviceItemView(string description)
        {
            Description = description;
            CreateLayout();
        }


        private void CreateLayout()
        {
            BackgroundColor = new Color(SystemColors.WindowBackground, 0.7f);
            Size = new Size(750, 85);

            var container = new TableLayout(1, 1)
            {
                Padding = new Padding(15)
            };

            var label = new Label
            {
                Font = new Font(SystemFont.Default, 14),
                Text = Description,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };

            container.Add(label, 0, 0, true, true);
            Content = container;
        }
    }
}
