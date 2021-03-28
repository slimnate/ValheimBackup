using System.ComponentModel;
using Newtonsoft.Json;

namespace ValheimBackup.BO
{
    /// <summary>
    /// Represents a pair of files (one being the source from the remote FTP
    /// server, the other being the local file where the backup was saved)
    /// and information associated with the files (name, time hash, extension)
    /// 
    /// Implements INotifyPropertyChanged interface and Newtonsoft.JSON serialization attributes
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BackupFilePair : INotifyPropertyChanged
    {
        private string _sourcePath;
        private string _destinationPath;
        private string _name;
        private string _timeHash;
        private string _extension;

        /// <summary>
        /// Name of the file (excluding full path, extension, and time hash)
        /// </summary>
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

        /// <summary>
        /// The time hash of the file pair, should be calculated from
        /// the date/time of backup creation, and be unique to each backup.
        /// This is what is used to ensure no naming conflicts between files
        /// for the same world/server.
        /// </summary>
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

        /// <summary>
        /// The file extension of the file pair
        /// </summary>
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

        /// <summary>
        /// Path to the source file (remote FTP path)
        /// </summary>
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

        /// <summary>
        /// Path to the local copy of the remote file
        /// </summary>
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

        /// <summary>
        /// Empty default constructor for JSON deserialization
        /// </summary>
        public BackupFilePair() { }

        /// <summary>
        /// Create a new BackupFilePair with the specified params
        /// </summary>
        /// <param name="source">source file path</param>
        /// <param name="destination">destination file path</param>
        /// <param name="name">file name</param>
        /// <param name="extension">file extension</param>
        /// <param name="timeHash">time hash</param>
        public BackupFilePair(string source, string destination, string name, string extension, string timeHash)
        {
            _sourcePath = source;
            _destinationPath = destination;
            _name = name;
            _extension = extension;
            _timeHash = timeHash;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
