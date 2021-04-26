using System;
using System.ComponentModel;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ValheimBackup.BO
{
    /// <summary>
    /// Represents an instance of a Backup object, which is associated with
    /// a single Server and a specific World on that server.
    /// 
    /// Used in the main UI to display the backup history to the user
    /// 
    /// Implements INotifyPropertyChanged interface and Newtonsoft.JSON serialization attributes
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Backup : INotifyPropertyChanged
    {
        private long _id;
        private long _serverId;
        private Server _server;
        private string _worldName;
        private DateTime _backupTime;
        private List<BackupFilePair> _files;

        /// <summary>
        /// Represents the Id for the backup instance.
        /// Should be derived from the timestamp of the backup to prevent
        /// conflicts, but does not necessarily represent the actual time
        /// that the backup was processed.
        /// </summary>
        [JsonProperty]
        public long Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        /// <summary>
        /// Represents the Id of the server that this backup was created from.
        /// Used to associate the backup with the correct server.
        /// </summary>
        [JsonProperty]
        public long ServerId
        {
            get => _serverId;
            set
            {
                if(_serverId != value)
                {
                    _serverId = value;
                    NotifyPropertyChanged("ServerId");
                }
            }
        }

        /// <summary>
        /// The name of the world this backup isntance is for.
        /// </summary>
        [JsonProperty]
        public string WorldName
        {
            get => _worldName;
            set
            {
                if (_worldName != value)
                {
                    _worldName = value;
                    NotifyPropertyChanged("WorldName");
                }
            }
        }

        /// <summary>
        /// Date and time the backup was initiated at.
        /// </summary>
        [JsonProperty]
        public DateTime BackupTime
        {
            get => _backupTime;
            set
            {
                if (_backupTime != value)
                {
                    _backupTime = value;
                    NotifyPropertyChanged("BackupTime");
                }
            }
        }

        /// <summary>
        /// A list of all the files associated with this backup instance
        /// </summary>
        [JsonProperty]
        public List<BackupFilePair> Files
        {
            get => _files;
            set
            {
                if(_files != value)
                {
                    _files = value;
                    NotifyPropertyChanged("Files");
                }
            }
        }

        public Server Server
        {
            get => _server;
            set
            {
                if(_server != value)
                {
                    _server = value;
                    NotifyPropertyChanged("Server");
                }
            }
        }

        public string FileCount
        {
            get
            {
                return Files.Count + " files";
            }
        }

        /// <summary>
        /// Empty default constructor for JSON deserialization
        /// </summary>
        public Backup() { }

        /// <summary>
        /// Create a new Backup with the specified parameters. This constructor
        /// should be used by the backup logic.
        /// </summary>
        /// <param name="serverId">Id of the server the backup was created from</param>
        /// <param name="world">World name for the backup</param>
        /// <param name="time">Date/time the backup was initiated</param>
        /// <param name="id">(optional) backup id - defaults to: <code>DateTime.Now.ToBinary()</code></param>
        public Backup(long serverId, string world, DateTime time, long id = -1L)
        {
            if (id == -1L)
            {
                id = DateTime.Now.ToBinary();
            }
            _id = id;
            _serverId = serverId;
            _worldName = world;
            _backupTime = time;
            _files = new List<BackupFilePair>();
        }

        /// <summary>
        /// Adds a new BackupFilePair to this backup instance. Used by the backup
        /// builder to associate each file with the correct backup instance.
        /// </summary>
        /// <param name="pair"></param>
        public void AddFile(BackupFilePair pair)
        {
            _files.Add(pair);
            NotifyPropertyChanged("Files");
        }

        #region INotifyPropertychanged

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
