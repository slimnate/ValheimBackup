using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimBackup.BO
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Server : INotifyPropertyChanged
    {
        private long _id = -1;
        private string _name;
        private string _description;

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
                if(_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

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

        [JsonProperty]
        public FtpConnectionInfo ConnectionInfo { get; set; }

        [JsonProperty]
        public BackupSettings BackupSettings { get; set; }

        public List<Backup> Backups { get; set; }

        //computed properties
        public string NameAndDescription
        {
            get
            {
                return Name + " - " + Description;
            }
        }

        public Server(string name, string desc, FtpConnectionInfo connectionInfo, BackupSettings backupSettings)
        {
            this.Name = name;
            this.Description = desc;
            this.ConnectionInfo = connectionInfo;
            this.BackupSettings = backupSettings;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            return Name +  " - " + ConnectionInfo;
        }
    }
}
