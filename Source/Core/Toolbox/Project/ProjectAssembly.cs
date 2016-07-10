using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace XenForms.Core.Toolbox.Project
{
    public class ProjectAssemblyComparer : IEqualityComparer<ProjectAssembly>
    {
        public bool Equals(ProjectAssembly x, ProjectAssembly y)
        {
            return ReferenceEquals(x, y) || x.FullPath.Equals(y.FullPath);
        }

        public int GetHashCode(ProjectAssembly obj)
        {
            return obj.FullPath.GetHashCode();
        }
    }


    public class ProjectAssembly : INotifyPropertyChanged
    {
        private bool _include;
        private string _fileName;
        private string _fullPath;
        private string _relativePath;
        private string _directory;
        private bool _sent;


        [JsonProperty("include")]
        public bool Include
        {
            get { return _include; }
            set
            {
                if (value == _include) return;
                _include = value;
                OnPropertyChanged();
            }
        }


        [JsonIgnore]
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (value == _fileName) return;
                _fileName = value;
                OnPropertyChanged();
            }
        }


        [JsonIgnore]
        public string FullPath
        {
            get { return _fullPath; }
            set
            {
                if (value == _fullPath) return;
                _fullPath = value;
                OnPropertyChanged();
            }
        }


        [JsonIgnore]
        public string Directory
        {
            get { return _directory; }
            set
            {
                if (value == _directory) return;
                _directory = value;
                OnPropertyChanged();
            }
        }


        [JsonProperty("relativePath")]
        public string RelativePath
        {
            get { return _relativePath; }
            set
            {
                if (value == _relativePath) return;
                _relativePath = value;
                OnPropertyChanged();
            }
        }


        [JsonIgnore]
        public bool Sent
        {
            get { return _sent; }
            set
            {
                if (value == _sent) return;
                _sent = value;
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