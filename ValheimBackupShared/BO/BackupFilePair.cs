using System.ComponentModel;
using Newtonsoft.Json;

namespace ValheimBackup.BO
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BackupFilePair : INotifyPropertyChanged
    {
        private string _sourcePath;
        private string _destinationPath;
        private string _name;
        private string _timeHash;
        private string _extension;

        [JsonProperty]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        [JsonProperty]
        public string TimeHash
        {
            get => _timeHash;
            set
            {
                if (_timeHash != value)
                {
                    _timeHash = value;
                    NotifyPropertyChanged("TimeHash");
                }
            }
        }

        [JsonProperty]
        public string Extension
        {
            get => _extension;
            set
            {
                if (_extension != value)
                {
                    _extension = value;
                    NotifyPropertyChanged("Extension");
                }
            }
        }

        [JsonProperty]
        public string SourcePath
        {
            get => _sourcePath;
            set
            {
                if (_sourcePath != value)
                {
                    _sourcePath = value;
                    NotifyPropertyChanged("SourcePath");
                }
            }
        }

        [JsonProperty]
        public string DestinationPath
        {
            get => _destinationPath;
            set
            {
                if (_destinationPath != value)
                {
                    _destinationPath = value;
                    NotifyPropertyChanged("DestinationPath");
                }
            }
        }

        public BackupFilePair() { }

        public BackupFilePair(string source, string destination, string name, string extension, string timeHash)
        {
            _sourcePath = source;
            _destinationPath = destination;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
