using System;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;

namespace XenForms.Toolbox.UI.Shell
{
    public class ButtonMenuList : Panel
    {
        public Image Image { get; set; }
        private readonly ContextMenu _menu = new ContextMenu();


        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);

            var button = new ImageButton
            {
                Width = Width,
                Image = Image,
                Height = Height
            };

            Content = button;
        
            button.Click += (a,b) =>
            {
                _menu.Show(this);
            };
        }


        public void AddCheckItem(string menuText, bool value, Action<bool> action)
        {
            var cmd = new CheckCommand(OnCheckCommand)
            {
                Checked = value,
                MenuText = menuText,
                Tag = action
            };

            var item = new CheckMenuItem(cmd);

            _menu.Items.Add(item);
        }


        public void Add(Command command)
        {
            _menu.Items.Add(command);
        }


        public void Add(MenuItem menuItem)
        {
            _menu.Items.Add(menuItem);
        }


        public void AddRange(IEnumerable<MenuItem> menuItems)
        {
            _menu.Items.AddRange(menuItems);
        }


        private void OnCheckCommand(object sender, EventArgs e)
        {
            var cmd = sender as CheckCommand;
            if (cmd == null) return;

            var action = cmd.Tag as Action<bool>;
            action?.Invoke(cmd.Checked);
        }
    }
}
