using System;
using System.ComponentModel;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ValheimBackup.BO
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Backup : INotifyPropertyChanged
    {
        private long _id = -1;
        private long _serverId;
        private string _worldName;
        private DateTime _backupTime;
        private List<BackupFilePair> _files;

        [JsonProperty]
        public long Id
        {
            get
            {
                if (_id == -1)
                {
                    _id = DateTime.Now.ToBinary();
                }
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

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

        /// <summary>
        /// Empty default constructor for deserialization
        /// </summary>
        public Backup() { }

        public Backup(long serverId, string world, DateTime time)
        {
            _serverId = serverId;
            _worldName = world;
            _backupTime = time;
            _files = new List<BackupFilePair>();
        }

        public void AddFile(BackupFilePair pair)
        {
            _files.Add(pair);
            NotifyPropertyChanged("Files");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static string TimeHash(DateTime dt)
        {
            return "-" + dt.ToString("yyyyMMddHHmmss");
        }
    }
}
