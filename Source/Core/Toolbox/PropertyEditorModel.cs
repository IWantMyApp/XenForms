using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using XenForms.Core.Widgets;

namespace XenForms.Core.Toolbox
{
    public class PropertyEditorModel<T> : IPropertyEditorModel, INotifyPropertyChanged
    {
        public Type ToolboxType => typeof(T);
        public bool UseDefaultValue { get; set; }
        public bool IsInitialized => UpdateCount > 0;

        public bool IsNull { get; set; }
        public bool SaveInBase64 { get; set; }
        public int UpdateCount { get; private set; }

        public string DisplayName { get; set; }
        public VisualTreeNode VisualNode { get; set; }
        public XenProperty Property { get; set; }

        public event EventHandler<PropertyEditorSavingEventArgs> Saving;


        private string _shortTypeName;
        public string ShortTypeName
        {
            get { return _shortTypeName; }
            set
            {
                if (Equals(value, _shortTypeName)) return;
                _shortTypeName = value;
                OnPropertyChanged();
            }
        }


        private object _toolboxValue;
        public object ToolboxValue
        {
            get { return _toolboxValue; }
            set
            {
                if (value != null && _toolboxValue != null)
                {
                    if (Equals(value, _toolboxValue)) return;
                }

                _toolboxValue = value;
                UpdateCount++;

                if (UpdateCount > 1)
                {
                    Save(value);
                }

                OnPropertyChanged();
            }
        }


        private string _fullTypeName;
        public string FullTypeName
        {
            get { return _fullTypeName; }
            set
            {
                if (Equals(value, _fullTypeName)) return;
                _fullTypeName = value;
                OnPropertyChanged();
            }
        }


        private object _displayValue;
        public object DisplayValue
        {
            get
            {
                if (_displayValue == null)
                {
                    return ToolboxValue;
                }

                return _displayValue;
            }
            set
            {
                if (Equals(value, _displayValue)) return;
                _displayValue = value;
                OnPropertyChanged();
            }
        }


        public virtual void Save(object value)
        {
            ToolboxValue = value;

            var args = new PropertyEditorSavingEventArgs
            {
                Metadata = Property.Metadata,
                IsBase64 = SaveInBase64,
                Node = VisualNode,
                Property = Property,
                ToolboxValue = ToolboxValue
            };

            OnSave(args);
        }


        protected virtual void OnSave(PropertyEditorSavingEventArgs e)
        {
            Saving?.Invoke(this, e);
        }


        #region PropertyChanged


        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion
    }
}