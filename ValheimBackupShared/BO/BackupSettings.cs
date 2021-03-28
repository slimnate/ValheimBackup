using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ValheimBackupShared.Properties;

namespace ValheimBackup.BO
{
    /// <summary>
    /// All or only specific worlds
    /// </summary>
    public enum WorldSelection : int
    {
        All, Specific
    }

    /// <summary>
    /// Represents all the information about how, when, where, and what worlds
    /// to backup for a specific server.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BackupSettings : INotifyPropertyChanged
    {
        private string _worldDirectory;
        private string _backupDirectory;
        private WorldSelection _worldSelection;
        private List<string> _selectedWorlds;

        /// <summary>
        /// The remote directory on this server containing the world files
        /// </summary>
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

        /// <summary>
        /// The local directory to save backup files in
        /// </summary>
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

        /// <summary>
        /// Determines whether to backup all worlds or specific worlds only
        /// </summary>
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

        /// <summary>
        /// Contains a list of the worlds that should be backed up.
        /// This property will be ignored unless the user has specifically
        /// selected to only backup specific worlds.
        /// </summary>
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

        /// <summary>
        /// The schedule for performing remote file backups
        /// </summary>
        [JsonProperty]
        public BackupSchedule Schedule { get; set; }

        /// <summary>
        /// The schedule for performing backup file cleanup
        /// </summary>
        [JsonProperty]
        public CleanupFrequency CleanupSchedule { get; set; }

        /// <summary>
        /// Returns a new instance of Backup settings with default properties.
        /// Defaults:
        /// Backup schedule: every 30 minutes
        /// World directory: /save/worlds
        /// Backup directory: read from application settings.
        /// World selection: All
        /// Cleanup schedule: Every 10 copies
        /// </summary>
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

        /// <summary>
        /// A list of file extensions that are valid for world servers
        /// </summary>
        private static readonly string[] VALID_EXTENSIONS = new string[] { ".db", ".fwl" };

        /// <summary>
        /// Empty default constructor for JSON deserialization
        /// </summary>
        public BackupSettings()  { }

        /// <summary>
        /// Determines whether a file should be backed up, based on it's name
        /// and extension.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="extension"></param>
        /// <returns>True if the file should be backed up, false otherwise.</returns>
        public bool ShouldBackup(string fileName, string extension)
        {
            if (WorldSelection == WorldSelection.Specific)
            {
                if (!SelectedWorlds.Contains(fileName))
                {
                    //specific worlds enabled, and not in selected worlds.
                    return false;
                }
            }
            if (!VALID_EXTENSIONS.Contains(extension))
            {
                // wrong extension
                return false;
            }

            // no checks failed, return true
            return true;
        }

        /// <summary>
        /// Overrides ToString to provide human readable representation of settings.
        /// </summary>
        /// <returns>"backing up {WorldSelection} worlds {Schedule}"</returns>
        public override string ToString()
        {
            return "backing up " + WorldSelection.ToString() + " worlds " + Schedule.ToString();
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
