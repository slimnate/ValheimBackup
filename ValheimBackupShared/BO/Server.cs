using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ValheimBackup.BO
{
    /// <summary>
    /// Represents a Server and all the info needed to perform backups for it.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Server : INotifyPropertyChanged
    {
        private long _id;
        private string _name;
        private string _description;

        /// <summary>
        /// The servers ID, derived as the binary representation of the current
        /// DateTime. Used to associate all of this servers Backup instances
        /// with it. Should be a unique value among each server.
        /// </summary>
        [JsonProperty]
        public long Id
        {
            get => _id;
            set
            {
                if(_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        /// <summary>
        /// Name of the server
        /// </summary>
        [JsonProperty]
        public string Name {
            get => _name;
            set
            {
                if(_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Description of the server
        /// </summary>
        [JsonProperty]
        public string Description {
            get => _description;
            set
            {
                if(_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        /// <summary>
        /// Connection details for the servers FTP connection
        /// </summary>
        [JsonProperty]
        public FtpConnectionInfo ConnectionInfo { get; set; }

        /// <summary>
        /// Settings for server backup process
        /// </summary>
        [JsonProperty]
        public BackupSettings BackupSettings { get; set; }

        /// <summary>
        /// TODO: Computed property that gets all the backup files associated with this server.
        /// </summary>
        public List<Backup> Backups { get; set; }

        /// <summary>
        /// Computed property that concatenations server name and description.
        /// </summary>
        public string NameAndDescription
        {
            get
            {
                return Name + " - " + Description;
            }
        }

        /// <summary>
        /// Create a new Server from specified params.
        /// </summary>
        /// <param name="name">server name</param>
        /// <param name="desc">server description</param>
        /// <param name="connectionInfo">FTP connection info</param>
        /// <param name="backupSettings">backup settings</param>
        /// <param name="id">(optional) server id - defaults to: <code>DateTime.Now.ToBinary()</code></param>
        public Server(string name, string desc, FtpConnectionInfo connectionInfo, BackupSettings backupSettings, long id = -1L)
        {
            if (id == -1)
            {
                id = DateTime.Now.ToBinary();
            }
            _id = id;
            this.Name = name;
            this.Description = desc;
            this.ConnectionInfo = connectionInfo;
            this.BackupSettings = backupSettings;
        }

        /// <summary>
        /// Override ToString to return human readable server details.
        /// </summary>
        /// <returns>"{Name} - {ConnectionInfo}"</returns>
        public override string ToString()
        {
            return Name +  " - " + ConnectionInfo;
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
