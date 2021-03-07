using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValheimBackup.FTP;
using Newtonsoft.Json;

namespace ValheimBackup.BO
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Backup : INotifyPropertyChanged
    {
        private long _id = -1;
        private long _serverId;
        private string _worldName;
        private string _sourcePath;
        private string _destinationPath;
        private DateTime _backupTime;

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

        [JsonProperty]
        public DateTime BackupTime
        {
            get => _backupTime;
            set
            {
                if (_backupTime != value)
                {
                    _backupTime = value;
                    NotifyPropertyChanged("DestinationPath");
                }
            }
        }

        public Backup(Server server, FtpFileInfo fileInfo)
        {
            _serverId = server.Id;
            _worldName = fileInfo.Name;
            _sourcePath = Path.Combine(server.BackupSettings.WorldDirectory, fileInfo.FullName);
            _backupTime = DateTime.Now;

            var backupFileName = fileInfo.Name + TimeHash(_backupTime) + fileInfo.Extension;
            _destinationPath = Path.Combine(server.BackupSettings.BackupDirectory, backupFileName);
        }

        public static string TimeHash(DateTime dt)
        {
            return "-" + dt.ToString("yyyyMMddHHmmss");
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
