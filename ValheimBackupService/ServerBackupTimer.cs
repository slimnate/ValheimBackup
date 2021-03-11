using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ValheimBackup.BO;
using ValheimBackup.Utils;

namespace ValheimBackupService
{
    public class ServerBackupTimer : Timer
    {
        private Server _server = null;
        public Server Server
        {
            get => _server;
        }
        public ServerBackupTimer(Server server)
        {
            _server = server;
            Interval = TimePeriod.ToSeconds(server.BackupSettings.Schedule.Frequency);
            AutoReset = true;
        }
    }
}
