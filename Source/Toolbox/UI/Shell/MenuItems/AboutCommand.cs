using System;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core;
using XenForms.Toolbox.UI.Resources;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class AboutCommand : Command
    {
        private Dialog _dialog;


        public AboutCommand()
        {
            MenuText = AboutResource.Menu_label;
            Shortcut = Keys.F11;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            DefineLayout();
            _dialog.ShowModal(Application.Instance.MainForm);
        }


        public void DefineLayout()
        {
            var close = new Button { Text = "Close" };

            var supportedTypes = new SupportedTypesView();
            supportedTypes.Activate();

            var logo = new Label
            {
                Text = "XenForms",
                Font = new Font(SystemFont.Default, 36f)
            };

            var topContents = new TableLayout(2, 1);
            topContents.Add(logo, 0, 0, false, true);
            topContents.Add(null, 1, 0, true, false);

            var top = new Panel
            {
                Padding = new Padding(10, 0, 0, 10),
                BackgroundColor = Colors.White,
                Content = topContents
            };

            var toolboxVersion = new Label {Text = "Toolbox Version: " + XenFormsEnvironment.ToolboxVersion};
            var designerVersion = new Label {Text = "Supports Designer Version: " + XenFormsEnvironment.DesignerVersion};
            var androidVersion = new Label {Text = "Android 4.0.3 - Android 6.0" };

            var right = new StackLayout
            {
                Orientation = Orientation.Vertical
            };

            right.Items.Add(toolboxVersion);
            right.Items.Add(designerVersion);
            right.Items.Add(androidVersion);

            var left = new StackLayout
            {
                Orientation = Orientation.Vertical
            };

            var link = new LinkButton
            {
                Text = "https://github.com/michaeled/XenForms"
            };

            var copyright = new Label
            {
                Text = "XenForms\nfrom Michael Davis",
                Wrap = WrapMode.Word
            };

            left.Items.Add(copyright);
            left.Items.Add(link);

            var description = new TableLayout(3, 1)
            {
                Padding = new Padding(0, 10, 0, 10)
            };

            description.Add(left, 0, 0, false, false);
            description.Add(null, 1, 0, true, false);
            description.Add(right, 2, 0, false, false);

            var ossThanks = new Label
            {
                Wrap = WrapMode.Word,
                Text = "Bug and feature requests can be reported through the Feedback dialog. Please include your log file with the bug report.\nXenForms was made possible by many free and open-source software packages. The full list can be found below."
            };

            var tabs = new TabControl();

            var pluginsTab = new TabPage
            {
                Text = "Plugins",
                Content = new PluginsGridView()
            };

            var ossWeb = new WebView();
            ossWeb.LoadHtml(AboutResource.Licenses);

            var ossTab = new TabPage
            {
                Text = "Open Source Software",
                Content = ossWeb
            };

            var editorsTab = new TabPage
            {
                Text = "Supported Type Editors",
                Content = supportedTypes.View
            };

            tabs.Pages.Add(pluginsTab);
            tabs.Pages.Add(editorsTab);
            tabs.Pages.Add(ossTab);

            var footer = new TableLayout(2, 1);
            footer.Add(null, 0, 0, true, false);
            footer.Add(close, 1, 0, false, false);

            var outter = new TableLayout(1,2);
            var inner = new TableLayout(1, 4)
            {
                Padding = new Padding(10),
                Spacing = new Size(0, 15)
            };

            inner.Add(description, 0, 0, false, false);
            inner.Add(ossThanks, 0, 1, true, false);
            inner.Add(tabs, 0, 2, true, true);
            inner.Add(footer, 0, 3, false, false);

            outter.Add(top, 0, 0, true, false);
            outter.Add(inner, 0, 1, true, true);

            _dialog = new Dialog
            {
                Icon = AppImages.Xf,
                Title = "About",
                Content = outter,
                ClientSize = new Size(750, 600),
                AbortButton = close
            };

            link.Click += (sender, args) => Application.Instance.Open("https://github.com/michaeled/XenForms/");
            close.Click += (sender, args) => _dialog.Close();
        }
    }
}
