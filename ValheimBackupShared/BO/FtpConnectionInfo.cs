using Newtonsoft.Json;
using System.ComponentModel;

namespace ValheimBackup.BO
{
    /// <summary>
    /// Represents connection details for a Server's FTP connection
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class FtpConnectionInfo : INotifyPropertyChanged
    {
        private string _host;
        private string _port;
        private string _username;
        private string _password;

        /// <summary>
        /// Host address of the server
        /// </summary>
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

        /// <summary>
        /// Port to connect to.
        /// </summary>
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

        /// <summary>
        /// Username credential
        /// </summary>
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

        /// <summary>
        /// Password credential
        /// </summary>
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

        /// <summary>
        /// Full connection URI
        /// Format: "{Host}:{Port}"
        /// </summary>
        public string FullUri
        {
            get
            {
                return Host + ":" + Port;
            }
        }

        /// <summary>
        /// Empty default constructor for JSON deserialization
        /// </summary>
        public FtpConnectionInfo() { }

        /// <summary>
        /// Create a new FtpConnectionInfo with the specified details.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public FtpConnectionInfo(string host, string port, string username, string password)
        {
            this.Host = host;
            this.Port = port;
            this.Username = username;
            this.Password = password;
        }

        /// <summary>
        /// Override to string to return host and port
        /// </summary>
        /// <returns>"{Host}:{Port}"</returns>
        public override string ToString()
        {
            return Host + ":" + Port;
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
