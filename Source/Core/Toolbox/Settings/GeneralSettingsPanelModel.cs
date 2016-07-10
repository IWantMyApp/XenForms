using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XenForms.Core.Toolbox.Settings
{
    public class GeneralSettingsPanelModel : INotifyPropertyChanged
    {
        private bool _showConnectOnStartup;
        private bool _enableTraceLogging;
        private string _defaultProjectDirectory;
        private string _xamarinStudioDirectory;
        private string _adbLocation;


        public bool ShowConnectOnStartup
        {
            get { return _showConnectOnStartup; }
            set
            {
                if (_showConnectOnStartup == value)
                {
                    return;
                }

                _showConnectOnStartup = value;
                OnPropertyChanged();
            }
        }


        public bool EnableTraceLogging
        {
            get { return _enableTraceLogging; }
            set
            {
                if (_enableTraceLogging == value)
                {
                    return;
                }

                _enableTraceLogging = value;
                OnPropertyChanged();
            }
        }


        public string DefaultProjectDirectory
        {
            get { return _defaultProjectDirectory; }
            set
            {
                if (_defaultProjectDirectory == value)
                {
                    return;
                }

                _defaultProjectDirectory = value;
                OnPropertyChanged();
            }
        }


        public string XamarinStudioDirectory
        {
            get { return _xamarinStudioDirectory; }
            set
            {
                if (_xamarinStudioDirectory == value)
                {
                    return;
                }

                _xamarinStudioDirectory = value;
                OnPropertyChanged();
            }
        }


        public string AdbLocation
        {
            get { return _adbLocation; }
            set
            {
                if (_adbLocation == value)
                {
                    return;
                }

                _adbLocation = value;
                OnPropertyChanged();
            }
        }


        public string OriginalAdbLocation { get; set; }


        public bool ShowRestartMessage
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AdbLocation)) return false;
                if (OriginalAdbLocation != AdbLocation) return true;

                return false;
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