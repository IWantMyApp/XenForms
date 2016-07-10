using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Widgets;
using XenForms.Toolbox.UI.Pouches.Properties.Cells;
using XenForms.Toolbox.UI.Pouches.Properties.Cells.Numeric;
using XenForms.Toolbox.UI.Pouches.Properties.Cells.String;
using XenForms.Toolbox.UI.Pouches.Properties.Cells.Object;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Pouches.Properties
{
    public sealed class PropertyEditorGridView : GridView
    {
        public int TypeNameColumnIndex { get; private set; }
        public FilterCollection<IPropertyEditorModel> Editors { get; }


        public PropertyEditorGridView()
        {
            Editors = new FilterCollection<IPropertyEditorModel>();
            Configure();
        }


        public void Clear()
        {
            Editors.Clear();
        }


        public bool ShowEditors(VisualTreeNode node, XenConstructor ctor)
        {
            try
            {
                Clear();
                var editors = new List<IPropertyEditorModel>();

                foreach (var p in ctor.Parameters.OrderBy(p => p.ParameterName))
                {
                    var model = ToolboxApp.Editors.Create(p);

                    if (model == null)
                    {
                        ToolboxApp.Log.Error($"Error creating {p.ShortTypeName} property editor.");
                        continue;
                    }

                    p.Value = null;
                    model.VisualNode = node;
                    editors.Add(model);

                    var captured = p;
                    model.Saving += (sender, args) =>
                    {
                        captured.Value = args.ToolboxValue;
                    };
                }

                foreach (var editor in editors)
                {
                    Editors.Add(editor);
                }

                DataStore = Editors;

                if (Editors.Any())
                {
                    ScrollToRow(0);
                }
            }
            catch (Exception e)
            {
                ToolboxApp.Log.Error(e, $"Error while showing property editors for {node.DisplayName}.");
                return false;
            }

            return true;
        }


        public bool ShowEditors(VisualTreeNode node, IEnumerable<XenProperty> properties = null, bool editable = true)
        {
            try
            {
                Clear();

                var props = properties ?? node.Widget.Properties;
                if (properties != null && !editable)
                {
                    props = properties.Select(p =>
                    {
                        var clone = p.ShallowClone();
                        clone.CanWrite = false;
                        return clone;
                    }).ToArray();
                }

                var editors = CreateEditors(node, props);
                foreach (var editor in editors)
                {
                    Editors.Add(editor);
                }

                DataStore = Editors;

                if (Editors.Any())
                {
                    ScrollToRow(0);
                }
            }
            catch (Exception e)
            {
                ToolboxApp.Log.Error(e, $"Error while showing property editors for {node.DisplayName}.");
                return false;
            }

            return true;
        }


        private void Configure()
        {
            // name column
            var nameBinding = Binding.Property((IPropertyEditorModel m) => m.DisplayName);
            var stringBinding = Binding.Delegate((object t) => (object) typeof(string));

            var nameCellTypes = new PropertyCell
            {
                TypeBinding = stringBinding,
                Types =
                {
                    new ReadOnlyCell { ItemBinding = nameBinding },
                }
            };

            var nameColumn = new GridColumn
            {
                Sortable = false,
                HeaderText = "Name",
                Editable = false,
                DataCell = nameCellTypes
            };

            // type name column
            var typeNameBinding = Binding.Property((IPropertyEditorModel m) => m.ShortTypeName);
            var typeNameCellTypes = new PropertyCell
            {
                TypeBinding = stringBinding,
                Types =
                {
                    new ReadOnlyCell { ItemBinding = typeNameBinding },
                }
            };

            var typeNameColumn = new GridColumn
            {
                Sortable = false,
                HeaderText = "Type",
                Editable = false,
                DataCell = typeNameCellTypes
            };

            // value column
            var valueBinding = Binding.Property((IPropertyEditorModel m) => m.ToolboxValue);
            var valueCellTypes = new PropertyCell
            {
                TypeBinding = Binding.Property((IPropertyEditorModel m) => (object) m.ToolboxType),
                Types =
                {
                    new NumberCell<byte> { ItemBinding = valueBinding.OfType<byte>() },
                    new NumberCell<byte?>(false, false, true) { ItemBinding = valueBinding.OfType<byte?>() },

                    new NumberCell<short> { ItemBinding = valueBinding.OfType<short>() },
                    new NumberCell<short?>(false, false, true) { ItemBinding = valueBinding.OfType<short?>() },

                    new NumberCell<ushort>(false, true) { ItemBinding = valueBinding.OfType<ushort>() },
                    new NumberCell<ushort?>(false, true, true) { ItemBinding = valueBinding.OfType<ushort?>() },

                    new NumberCell<int> { ItemBinding = valueBinding.OfType<int>() },
                    new NumberCell<int?>(false, false, true) { ItemBinding = valueBinding.OfType<int?>() },

                    new NumberCell<uint>(false, true) { ItemBinding = valueBinding.OfType<uint>() },
                    new NumberCell<uint?>(false, true, true) { ItemBinding = valueBinding.OfType<uint?>() },

                    new NumberCell<long> { ItemBinding = valueBinding.OfType<long>() },
                    new NumberCell<long?>(false, false, true) { ItemBinding = valueBinding.OfType<long?>() },

                    new NumberCell<ulong>(false, true) { ItemBinding = valueBinding.OfType<ulong>() },
                    new NumberCell<ulong?>(false, true, true) { ItemBinding = valueBinding.OfType<ulong?>() },

                    new NumberCell<float>(true) { ItemBinding = valueBinding.OfType<float>() },
                    new NumberCell<float?>(true, false, true) { ItemBinding = valueBinding.OfType<float?>() },

                    new NumberCell<double>(true) { ItemBinding = valueBinding.OfType<double>() },
                    new NumberCell<double?>(true, false, true) { ItemBinding = valueBinding.OfType<double?>() },

                    new CharCell { ItemBinding = valueBinding.OfType<char>() },
                    new BoolCell { ItemBinding = valueBinding.OfType<bool>() },

                    new GeneralStringCell { ItemBinding = valueBinding.OfType<string>() },
                    new ColorCell { ItemBinding = valueBinding.OfType<Color>() },
                    new PropertyCellTypeDateTime { ItemBinding = valueBinding.OfType<DateTime?>() },
                    new ObjectCell { ItemBinding = valueBinding.OfType<object>()}
                }
            };

            var valueColumn = new GridColumn
            {
                Width = 150,
                Resizable = false,
                Editable = true,
                HeaderText = "Value",
                DataCell = valueCellTypes
            };

            RowHeight = AppStyles.GridRowHeight;

            Columns.Add(nameColumn);
            Columns.Add(valueColumn);
            Columns.Add(typeNameColumn);

            TypeNameColumnIndex = Columns.IndexOf(typeNameColumn);
        }


        public IEnumerable<IPropertyEditorModel> CreateEditors(VisualTreeNode node, IEnumerable<XenProperty> properties)
        {
            var editors = new List<IPropertyEditorModel>();

            foreach (var property in properties.OrderBy(p => p.PropertyName))
            {
                var model = ToolboxApp.Editors.Create(property);

                if (model == null)
                {
                    ToolboxApp.Log.Error($"Error adding {property.XenType.FullName} type editor.");
                    continue;
                }

                model.VisualNode = node;
                editors.Add(model);

                model.Saving += (_, args) =>
                {
                    var widget = args.Node.Widget;
                    var isAp = args.Property.XenType.Descriptor.HasFlag(XenPropertyDescriptors.AttachedProperty);
                    
                    ToolboxApp.Designer.SetProperty(widget, args.Property, args.ToolboxValue, args.IsBase64, isAp, args.Metadata);
                };
            }

            return editors;
        }
    }
}
