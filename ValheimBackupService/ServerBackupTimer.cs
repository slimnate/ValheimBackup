using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ValheimBackup.BO;

namespace ValheimBackupService
{
    public class ServerBackupTimer : Timer
    {
        private static readonly int SECONDS = 1000;
        private static readonly int MINUTES = SECONDS * 60;
        private static readonly int HOURS = MINUTES * 60;
        private static readonly int DAYS = HOURS * 24;
        private static readonly int WEEKS = DAYS * 7;

        private Server _server = null;
        public Server Server
        {
            get => _server;
        }
        public ServerBackupTimer(Server server)
        {
            _server = server;
            Interval = GetInterval(server.BackupSettings.Schedule.Frequency);
            AutoReset = true;
        }

        private double GetInterval(BackupFrequency frequency)
        {
            return frequency.Amount * PeriodMultiplier(frequency.Period);
        }

        private double PeriodMultiplier(BackupPeriod period)
        {
            switch(period)
            {
                case BackupPeriod.Minutes:
                    return MINUTES;
                case BackupPeriod.Hours:
                    return HOURS;
                case BackupPeriod.Days:
                    return DAYS;
                case BackupPeriod.Weeks:
                    return WEEKS;
                default:
                    throw new Exception("Can't get multiplier - Invalid BackupPeriod - this should NEVER happen");
            }
        }
    }
}
