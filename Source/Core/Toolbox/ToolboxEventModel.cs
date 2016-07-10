using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XenForms.Core.Toolbox
{
    public class ToolboxEventModel : IToolboxEventModel, INotifyPropertyChanged
    {
        private string _eventHandlerName;
        private string _sourceFile;
        private string _displayName;
        private ToolboxEventVisibility _visibility;


        public string EventHandlerName
        {
            get { return _eventHandlerName; }
            set
            {
                if (_eventHandlerName == value) return;

                _eventHandlerName = value;
                OnPropertyChanged();
            }
        }


        public string SourceFile
        {
            get { return _sourceFile; }
            set
            {
                if (_sourceFile == value) return;

                _sourceFile = value;
                OnPropertyChanged();
            }
        }


        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (_displayName == value) return;
                _displayName = value;

                OnPropertyChanged();
            }
        }


        public ToolboxEventVisibility Visibility
        {
            get { return _visibility; }
            set
            {
                if (_visibility == value) return;

                _visibility = value;
                OnPropertyChanged();
            }
        }


        #region Property Changed


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion
    }
}
