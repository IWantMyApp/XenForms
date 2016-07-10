using System;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;

namespace XenForms.Toolbox.UI.Pouches.VisualTree
{
    public class VisualTreeCell : CustomCell
    {
        protected override Control OnCreateCell(CellEventArgs args)
        {
            base.OnCreateCell(args);
            var container = new TableLayout(1, 1)
            {
                Width = -1,
                Padding = new Padding(1)
            };

            var item = args.Item as TreeGridItem;
            var node = item?.Tag as VisualTreeNode;

            var label = new Label
            {
                Text = node?.DisplayName ?? "Unknown"
            };

            container.Add(label, 0, 0, true, false);
            return container;
        }


        protected override void OnConfigureCell(CellEventArgs args, Control control)
        {
            base.OnConfigureCell(args, control);

            var container = control as TableLayout;
            var label = container?.Rows[0].Cells[0].Control as Label;
            if (label == null) return;

            if (args.IsSelected)
            {
                container.BackgroundColor = SystemColors.Highlight;
                label.TextColor = SystemColors.HighlightText;
            }
            else
            {
                container.BackgroundColor = Colors.White;
                label.TextColor = SystemColors.ControlText;
            }
        }
    }
}
