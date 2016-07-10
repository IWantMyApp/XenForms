using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using Newtonsoft.Json;
using XenForms.Core.Widgets;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells
{
    public class ColorCell : PropertyEditorCell<Color>
    {
        public override Control OnCreate(CellEventArgs args)
        {
            if (ItemBinding == null)
            {
                return null;
            }

            Initialize(args);
            var model = Model;

            var dval = ItemBinding.GetValue(args.Item);
            var tval = Convert(model.Property);

            var picker = new ColorPicker
            {
                Value = tval == dval ? dval : tval
            };

            picker.ValueChanged += (sender, eventArgs) =>
            {
                model.ToolboxValue = picker.Value.ToString();
            };

            if (!Model.Property.CanWrite)
            {
                picker.Enabled = false;
            }

            var layout = new TableLayout(1, 1);
            layout.Add(picker, 0, 0, true, true);

            return layout;
        }


        public override void OnPaint(CellPaintEventArgs args)
        {
            // ignored
        }


        private Color Convert(XenProperty property)
        {
            var json = property.Value?.ToString();
            if (string.IsNullOrWhiteSpace(json)) return Colors.Transparent;

            var xenProps = JsonConvert.DeserializeObject<XenProperty[]>(json);

            var r = GetProperty(xenProps, "R");
            var g = GetProperty(xenProps, "G");
            var b = GetProperty(xenProps, "B");
            var a = GetProperty(xenProps, "A");

            if (r == -1 && g == -1 && b == -1 && a == -1)
            {
                return new Color(0,0,0,0);
            }

            return new Color(r, g, b, a);
        }


        private float GetProperty(IEnumerable<XenProperty> props, string name)
        {
            var strval = props.FirstOrDefault(p => p.PropertyName == name)?.Value as string;

            if (string.IsNullOrWhiteSpace(strval))
            {
                return -1;
            }

            float result;
            var parsed = float.TryParse(strval, out result);

            return parsed ? result : -1;
        }
    }
}
