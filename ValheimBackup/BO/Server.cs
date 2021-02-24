using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimBackup.BO
{
    public class Server
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public FtpConnectionInfo ConnectionInfo { get; set; }
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

        public override string ToString()
        {
            return Name +  " - " + ConnectionInfo;
        }
    }
}
