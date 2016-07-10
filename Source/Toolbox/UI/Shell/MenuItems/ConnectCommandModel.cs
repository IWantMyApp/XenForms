using System.ComponentModel;
using System.Runtime.CompilerServices;
using XenForms.Core.Networking;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.Shell;
using XenForms.Toolbox.UI.Properties;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class ConnectCommandModel : INotifyPropertyChanged
    {
        private readonly ISettingsStore _store;
        private string _host;
        private string _port;
        private bool _okEnabled;
        private bool _rememberSettings;
        private string _accessKey;


        public ConnectCommandModel(ISettingsStore store)
        {
            _store = store;
        }


        public string AccessKey
        {
            get { return _accessKey; }
            set
            {
                if (value == _accessKey) return;
                _accessKey = value;
                OnPropertyChanged();
            }
        }


        public string Host
        {
            get { return _host; }
            set
            {
                if (value == _host) return;
                _host = value;

                UpdateOkEnabled();
                OnPropertyChanged();
            }
        }


        public string Port
        {
            get { return _port; }
            set
            {
                if (value == _port) return;
                _port = value;

                UpdateOkEnabled();
                OnPropertyChanged();
            }
        }


        public bool OkEnabled
        {
            get { return _okEnabled; }
            set
            {
                if (value == _okEnabled) return;
                _okEnabled = value;
                OnPropertyChanged();
            }
        }


        public bool RememberSettings
        {
            get { return _rememberSettings; }
            set
            {
                if (value == _rememberSettings) return;
                _rememberSettings = value;
                OnPropertyChanged();
            }
        }


        public void Save()
        {
            if (!RememberSettings)
            {
                Port = string.Empty;
                Host = string.Empty;
            }

            _store.Set(UserSettingKeys.Builtin.RemoteHost, Host);
            _store.Set(UserSettingKeys.Builtin.RemotePort, Port);
            _store.Set(UserSettingKeys.Builtin.RememberHost, RememberSettings);
        }


        public void Restore()
        {
            var host = _store.GetString(UserSettingKeys.Builtin.RemoteHost);
            var port = _store.GetString(UserSettingKeys.Builtin.RemotePort);

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(port))
            {
                return;
            }

            Host = host;
            Port = port;
            RememberSettings = _store.GetBool(UserSettingKeys.Builtin.RememberHost) ?? false;
        }


        private void UpdateOkEnabled()
        {
            OkEnabled = !string.IsNullOrWhiteSpace(Host) && ServiceEndpoint.IsValidPort(Port);
        }

        
        #region Property Changed


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion
    }
}
