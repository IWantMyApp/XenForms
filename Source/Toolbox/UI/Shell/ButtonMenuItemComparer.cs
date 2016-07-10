using System;
using System.Collections.Generic;
using Eto.Forms;

namespace XenForms.Toolbox.UI.Shell
{
    public class ButtonMenuItemComparer : IComparer<ButtonMenuItem>
    {
        public static Func<MenuItem, string> Prepare = i => i.Text?.Trim('&') ?? string.Empty;


        public int Compare(ButtonMenuItem x, ButtonMenuItem y)
        {
            if (x == null || y == null) return 0;

            var xText = Prepare(x);
            var yText = Prepare(y);

            return string.Compare(xText, yText, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}