using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using XenForms.Core.Properties;

namespace XenForms.Core.Toolbox.Project
{
    public class ProjectView : INotifyPropertyChanged
    {
        private string _typeName;
        private bool _persisted;
        private bool _deleted;


        [JsonProperty("typeName")]
        public string TypeName
        {
            get { return _typeName; }
            set
            {
                if (value == _typeName) return;
                _typeName = value;
                OnPropertyChanged();
            }
        }


        [JsonIgnore]
        public bool Persisted
        {
            get { return _persisted; }
            set
            {
                if (value == _persisted) return;
                _persisted = value;
                OnPropertyChanged();
            }
        }


        [JsonIgnore]
        public bool Deleted
        {
            get { return _deleted; }
            set
            {
                if (value == _deleted) return;
                _deleted = value;
                OnPropertyChanged();
            }
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