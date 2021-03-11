﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValheimBackup.BO;

namespace ValheimBackup.Utils
{
    public static class TimePeriod
    {
        private static readonly int SECONDS = 1000;
        private static readonly int MINUTES = SECONDS * 60;
        private static readonly int HOURS = MINUTES * 60;
        private static readonly int DAYS = HOURS * 24;
        private static readonly int WEEKS = DAYS * 7;

        public static double ToSeconds(BackupFrequency frequency)
        {
            return frequency.Amount * Multiplier(frequency.Period);
        }

        public static double ToSeconds(CleanupFrequency frequency)
        {
            if (frequency.Period == CleanupPeriod.Copies)
            {
                throw new ArgumentException("frequency period is CleanupPeriod.Copies, cannot convert copies to seconds.");
            }
            return frequency.Amount * Multiplier(frequency.Period);
        }

        public static double Multiplier(BackupPeriod period)
        {
            switch (period)
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

        public static double Multiplier(CleanupPeriod period)
        {
            switch (period)
            {
                case CleanupPeriod.Minutes:
                    return MINUTES;
                case CleanupPeriod.Hours:
                    return HOURS;
                case CleanupPeriod.Days:
                    return DAYS;
                case CleanupPeriod.Weeks:
                    return WEEKS;
                default:
                    throw new Exception("Can't get multiplier - Invalid CleanupPeriod - this COULD be CleanupPeriod.Copies, or an invalid value");
            }
        }
    }
}
