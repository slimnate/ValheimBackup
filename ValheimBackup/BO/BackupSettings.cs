using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimBackup.BO
{
    /// <summary>
    /// Represents a time period (or copies for cleanup
    /// </summary>
    public enum BackupPeriod : int
    {
        Minutes, Hours, Days, Weeks
    }

    public enum CleanupPeriod : int
    {
        Minutes, Hours, Days, Weeks, Copies
    }

    /// <summary>
    /// All or only specific worlds
    /// </summary>
    public enum WorldSelection : int
    {
        All, Specific
    }

    /// <summary>
    /// Represents all the information about how and when to backup worlds for a specific server.
    /// </summary>
    public class BackupSettings
    {
        public BackupSchedule Schedule { get; set; }
        public DateTime LastBackup { get; set; }
        public string WorldDirectory { get; set; }
        public string BackupDirectory { get; set; }
        public WorldSelection WorldSelection { get; set; }
        public List<string> SelectedWorlds { get; set; }
        public CleanupFrequency CleanupSchedule { get; set; }

        public static BackupSettings Default
        {
            get
            {
                return new BackupSettings()
                {
                    Schedule = new BackupSchedule(new BackupFrequency(30, BackupPeriod.Minutes)),
                    WorldDirectory = "/save/worlds",
                    BackupDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "valheim_backup"),
                    WorldSelection = WorldSelection.All,
                    SelectedWorlds = new List<string>() { "world1", "world2" },
                    CleanupSchedule = new CleanupFrequency(10, CleanupPeriod.Copies)
                };
            }
        }

        public BackupSettings()
        {

        }

        public override string ToString()
        {
            return "backing up " + WorldSelection.ToString() + " worlds " + Schedule.ToString();
        }

        //TODO: add naming convention for backup files
    }

    /// <summary>
    /// Represents a schedule for backing up world files.
    /// </summary>
    public class BackupSchedule
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public BackupFrequency Frequency { get; set; }

        public BackupSchedule(BackupFrequency frequency)
            : this(frequency, DateTime.Now) { }

        public BackupSchedule(BackupFrequency frequency, DateTime startDate)
            : this(frequency, startDate, DateTime.Now.AddDays(365)) { }

        public BackupSchedule(BackupFrequency frequency, DateTime startDate, DateTime endDate)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Frequency = frequency;
        }

        public override string ToString()
        {
            return "every " + Frequency.Amount + " " + Frequency.Period.ToString() + " starting on " + StartDate.ToString();
        }
    }

    /// <summary>
    /// Represents a cleanup schedule for backed up world files. Determines how long to keep old copies.
    /// </summary>
    public class CleanupSchedule
    {
        public CleanupFrequency Frequency { get; set; }

        public CleanupSchedule(CleanupFrequency frequency)
        {
            this.Frequency = frequency;
        }
    }

    /// <summary>
    /// Represents the frequency of occurance for a backup event in the form of "every {amount} {unit}".
    /// eg. every 2 days, every 30 minutes, every 1 week
    /// </summary>
    public class BackupFrequency
    {
        public int Amount { get; set; }
        public BackupPeriod Period { get; set; }

        public BackupFrequency(int amount, BackupPeriod period)
        {
            this.Amount = amount;
            this.Period = period;
        }
    }

    /// <summary>
    /// Represents the frequency of occurance for cleanup events in the form of "every {amount} {unit}".
    /// eg. every 2 days, every 30 minutes, every 10 copies
    /// </summary>
    public class CleanupFrequency
    {
        public int Amount { get; set; }
        public CleanupPeriod Period { get; set; }

        public CleanupFrequency(int amount, CleanupPeriod period)
        {
            this.Amount = amount;
            this.Period = period;
        }
    }
}
