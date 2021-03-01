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
    public class FtpConnectionInfo : INotifyPropertyChanged
    {
        private string _host;
        private string _port;
        private string _username;
        private string _password;

        [JsonProperty]
        public string Host
        {
            get => _host;
            set
            {
                if (_host != value)
                {
                    _host = value;
                    NotifyPropertyChanged("Host");
                }
            }
        }

        [JsonProperty]
        public string Port
        {
            get => _port;
            set
            {
                if (_port != value)
                {
                    _port = value;
                    NotifyPropertyChanged("Port");
                }
            }
        }

        [JsonProperty]
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    NotifyPropertyChanged("Username");
                }
            }
        }

        [JsonProperty]
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    NotifyPropertyChanged("Password");
                }
            }
        }

        public string FullUri
        {
            get
            {
                return Host + ":" + Port;
            }
        }

        public FtpConnectionInfo(string host, string port, string username, string password)
        {
            this.Host = host;
            this.Port = port;
            this.Username = username;
            this.Password = password;
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
            return Host + ":" + Port;
        }
    }
}
