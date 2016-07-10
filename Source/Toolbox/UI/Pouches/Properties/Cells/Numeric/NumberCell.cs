using System;
using System.Globalization;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;

namespace XenForms.Toolbox.UI.Pouches.Properties.Cells.Numeric
{
    public class NumberCell<T> : PropertyEditorCell<T>
    {
        public bool AllowDecimals { get; }
        public bool Unsigned { get; }
        public bool Nullable { get; }

        private object Default { get; set; }


        public NumberCell(bool allowDecimals = false, bool unsigned = false, bool nullable = false)
        {
            AllowDecimals = allowDecimals;
            Unsigned = unsigned;
            Nullable = nullable;
        }


        public override Control OnCreate(CellEventArgs args)
        {
            Initialize(args);

            if (!IsNull(Model.ToolboxValue))
            {
                double number;

                var tv = Convert.ToString(Model.ToolboxValue, CultureInfo.InvariantCulture);
                var parsed = Double.TryParse(tv, NumberStyles.Any,
                    NumberFormatInfo.InvariantInfo, out number);

                if (parsed)
                {
                    Default = number;
                }
            }

            var layout = new TableLayout(2, 1);

            var numeric = new NumericUpDown
            {
                Increment = 1.0,
                DecimalPlaces = AllowDecimals ? 1 : 0,
                MaximumDecimalPlaces = 1
            };

            var chkbox = new CheckBox
            {
                ThreeState = false,
                Text = "null"
            };

            if (Unsigned)
            {
                numeric.MinValue = 0;
            }

            if (Model.Property.CanWrite == false)
            {
                chkbox.Enabled = false;
                numeric.ReadOnly = true;
            }

            // used by the layout to show or hide checkbox
            Control left = null;
            var @default = Default;

            numeric
                .ValueBinding
                .BindDataContext(Binding.Property((PropertyEditorModel<object> t) => t.ToolboxValue)
                .Convert(v => ConvertValue(v, @default), v => v));

            // capture model
            var model = Model;

            if (Nullable)
            {
                layout.Spacing = new Size(5, 0);

                chkbox.CheckedChanged += (s, e) =>
                {
                    if (!chkbox.Checked.HasValue) return;

                    // toggle enabled
                    numeric.Enabled = !chkbox.Checked.Value;

                    if (chkbox.Checked.Value)
                    {
                        // null is checked; send 'null' string to design surface.
                        model.ToolboxValue = "null";
                    }
                    else
                    {
                        // reset to default value
                        model.ToolboxValue = ConvertValue(model.ToolboxValue, @default);
                    }
                };

                left = chkbox;
            }

            layout.Add(left, 0, 0, false, true);
            layout.Add(numeric, 1, 0, true, true);
            return layout;
        }


        private bool IsNull(object value)
        {
            return value == null || value.Equals("null");
        }


        private double ConvertValue(object value, object @default)
        {
            try
            {
                if (!Model.UseDefaultValue)
                {
                    return 0;
                }

                if (IsNull(value))
                {
                    return Convert.ToDouble(@default);
                }

                double number;
                var parsed = Double.TryParse(Convert.ToString(Model.ToolboxValue, CultureInfo.InvariantCulture), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out number);

                if (parsed)
                {
                    return number;
                }

                return 0;
            }
            catch (Exception e)
            {
                ToolboxApp.Log.Error(e, $"Error converting value in {nameof(NumberCell<T>)} for {Model.DisplayName}.");
                return 0;
            }
        }


        public override void OnPaint(CellPaintEventArgs args){}
    }
}