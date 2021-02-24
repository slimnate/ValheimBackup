using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimBackup.BO
{
    /// <summary>
    /// Represents a time period for backups
    /// </summary>
    public enum BackupPeriod : int
    {
        Minutes, Hours, Days, Weeks
    }

    /// <summary>
    /// Represents a time period for cleanups
    /// </summary>
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
    public class BackupSettings : INotifyPropertyChanged
    {
        private DateTime _lastBackup;
        private string _worldDirectory;
        private string _backupDirectory;
        private WorldSelection _worldSelection;
        private List<string> _selectedWorlds;

        public DateTime LastBackup
        {
            get => _lastBackup;
            set
            {
                if(_lastBackup != value)
                {
                    _lastBackup = value;
                    NotifyPropertyChanged("LastBackup");
                }
            }
        }
        public string WorldDirectory
        {
            get => _worldDirectory;
            set
            {
                if (_worldDirectory != value)
                {
                    _worldDirectory = value;
                    NotifyPropertyChanged("WorldDirectory");
                }
            }
        }
        public string BackupDirectory
        {
            get => _backupDirectory;
            set
            {
                if (_backupDirectory != value)
                {
                    _backupDirectory = value;
                    NotifyPropertyChanged("BackupDirectory");
                }
            }
        }
        public WorldSelection WorldSelection
        {
            get => _worldSelection;
            set
            {
                if (_worldSelection != value)
                {
                    _worldSelection = value;
                    NotifyPropertyChanged("WorldSelection");
                }
            }
        }
        public List<string> SelectedWorlds
        {
            get => _selectedWorlds;
            set
            {
                if (_selectedWorlds != value)
                {
                    _selectedWorlds = value;
                    NotifyPropertyChanged("SelectedWorlds");
                }
            }
        }

        public BackupSchedule Schedule { get; set; }
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
            return "backing up " + WorldSelection.ToString() + " worlds " + Schedule.ToString();
        }

        //TODO: add naming convention for backup files
    }

    /// <summary>
    /// Represents a schedule for backing up world files.
    /// </summary>
    public class BackupSchedule : INotifyPropertyChanged
    {
        private DateTime? _startDate;
        private DateTime? _endDate;

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    NotifyPropertyChanged("StartDate");
                }
            }
        }
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    NotifyPropertyChanged("EndDate");
                }
            }
        }
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
            return "every " + Frequency.Amount + " " + Frequency.Period.ToString() + " starting on " + StartDate.ToString();
        }
    }

    /// <summary>
    /// Represents the frequency of occurance for a backup event in the form of "every {amount} {unit}".
    /// eg. every 2 days, every 30 minutes, every 1 week
    /// </summary>
    public class BackupFrequency : INotifyPropertyChanged
    {
        private int _amount;
        private BackupPeriod _period;
        
        public int Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    NotifyPropertyChanged("Amount");
                }
            }
        }
        public BackupPeriod Period
        {
            get => _period;
            set
            {
                if (_period != value)
                {
                    _period = value;
                    NotifyPropertyChanged("Period");
                }
            }
        }

        public BackupFrequency(int amount, BackupPeriod period)
        {
            this.Amount = amount;
            this.Period = period;
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

    /// <summary>
    /// Represents the frequency of occurance for cleanup events in the form of "every {amount} {unit}".
    /// eg. every 2 days, every 30 minutes, every 10 copies
    /// </summary>
    public class CleanupFrequency : INotifyPropertyChanged
    {
        private int _amount;
        private CleanupPeriod _period;

        public int Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    NotifyPropertyChanged("Amount");
                }
            }
        }
        public CleanupPeriod Period
        {
            get => _period;
            set
            {
                if (_period != value)
                {
                    _period = value;
                    NotifyPropertyChanged("Period");
                }
            }
        }

        public CleanupFrequency(int amount, CleanupPeriod period)
        {
            this.Amount = amount;
            this.Period = period;
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
