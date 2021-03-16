using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ValheimBackupShared.Properties;

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
    [JsonObject(MemberSerialization.OptIn)]
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

        [JsonProperty]
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

        [JsonProperty]
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

        [JsonProperty]
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

        [JsonProperty]
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

        [JsonProperty]
        public BackupSchedule Schedule { get; set; }

        [JsonProperty]
        public CleanupFrequency CleanupSchedule { get; set; }

        public static BackupSettings Default
        {
            get
            {
                return new BackupSettings()
                {
                    Schedule = new BackupSchedule(new BackupFrequency(30, BackupPeriod.Minutes)),
                    WorldDirectory = "/save/worlds",
                    BackupDirectory = Settings.Default.DefaultBackupDirectory,
                    WorldSelection = WorldSelection.All,
                    SelectedWorlds = new List<string>(),
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

        private static readonly string[] VALID_EXTENSIONS = new string[] { ".db", ".fwl" };
        public bool ShouldBackup(string fileName, string extension)
        {
            if (WorldSelection == WorldSelection.Specific) // specific worlds enabled
            {
                if(!SelectedWorlds.Contains(fileName)) // not in list
                {
                    return false;
                }
            }
            if(!VALID_EXTENSIONS.Contains(extension)) // wrong extension
            {
                return false;
            }

            return true; // true if not tests failed
        } 

        public override string ToString()
        {
            return "backing up " + WorldSelection.ToString() + " worlds " + Schedule.ToString();
        }

        //TODO: add naming convention for backup files
    }
}
