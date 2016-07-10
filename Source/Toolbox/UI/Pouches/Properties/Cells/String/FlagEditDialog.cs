using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Resources;
using XenForms.Toolbox.UI.Shell;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells.String
{
    public static class FlagEditDialog
    {
        public static Dialog Create(PropertyEditorModel<string> model)
        {
            var xt = model.Property?.XenType;
            var chks = new List<CheckBox>();

            var dialog = new ConnectedDialog
            {
                Title = "Edit Enumeration",
                Padding = AppStyles.WindowPadding,
                Width = 275,
                Height = 225
            };

            var footer = new TableLayout(3, 1)
            {
                Spacing = new Size(5, 0)
            };

            var layout = new TableLayout(1, 2)
            {
                Spacing = new Size(0, 10)
            };

            var chkStack = new StackLayout
            {
                Padding = AppStyles.PanelPadding,
                BackgroundColor = Colors.White,
                Spacing = 5,
                Orientation = Orientation.Vertical
            };

            // add checkboxes to stack.
            if (xt?.PossibleValues.Length > 0)
            {
                foreach (var pv in xt.PossibleValues)
                {
                    var chkd = false;
                    var value = (string) model.ToolboxValue;

                    var split = value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Any(s => s != null && s.Trim() == pv))
                    {
                        chkd = true;
                    }

                    var chk = new CheckBox
                    {
                        Tag = pv,
                        Checked = chkd,
                        Text = pv
                    };

                    // expand the width of the checkboxes so they're easier to click.
                    var item = new StackLayoutItem
                    {
                        Control = chk,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };

                    chkStack.Items.Add(item);

                    // make the checkboxes easier to access in the OkClicked event.
                    chks.Add(chk);
                }
            }
            else
            {
                ToolboxApp.Log.Warn($"No flags found for {model.DisplayName}.");
            }

            var scroll = new Scrollable
            {
                Border = BorderType.Bezel,
                Content = chkStack
            };

            var ok = new Button { Text = CommonResource.Ok };
            var cancel = new Button {Text = CommonResource.Cancel};

            footer.Add(null, 0, 0, true, false);
            footer.Add(ok, 1, 0, false, false);
            footer.Add(cancel, 2, 0, false, false);

            layout.Add(scroll, 0, 0, true, true);
            layout.Add(footer, 0, 1, true, false);

            dialog.Content = layout;

            ok.Click += (s, e) =>
            {
                var value = string.Empty;

                // build the toolbox value
                foreach (var chk in chks)
                {
                    // only include checked flags.
                    if (chk.Checked == true)
                    {
                        var tag = (string) chk.Tag;

                        // first flag
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            value = tag;
                        }
                        else
                        {
                            // other flags; note the spacing
                            value += $" | {tag}";
                        }
                    }
                }

                model.ToolboxValue = value;
                dialog.Close();
            };

            // discard changes
            cancel.Click += (s, e) => { dialog.Close(); };

            dialog.AbortButton = cancel;
            dialog.DefaultButton = ok;

            return dialog;
        }
    }
}