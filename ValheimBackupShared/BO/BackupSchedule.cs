using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace ValheimBackup.BO
{
    /// <summary>
    /// Represents a schedule for backing up world files.
    /// Encompasses start/end date-time, and frequency.
    /// 
    /// Implements INotifyPropertyChanged interface and Newtonsoft.JSON serialization attributes
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BackupSchedule : INotifyPropertyChanged
    {
        private DateTime? _startDate;
        private DateTime? _endDate;

        /// <summary>
        /// When to start backing up files
        /// </summary>
        [JsonProperty]
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

        /// <summary>
        /// When to stop backign up files
        /// </summary>
        [JsonProperty]
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

        /// <summary>
        /// How often to backup files
        /// </summary>
        [JsonProperty]
        public BackupFrequency Frequency { get; set; }

        /// <summary>
        /// Empty default constructor for JSON deserialization
        /// </summary>
        public BackupSchedule() { }

        /// <summary>
        /// Create a new BackupSchedule with specified frequency, and default start/end date.
        /// Defaults:
        /// <code>StartDate: DateTime.Now</code>
        /// <code>EndDate: DateTime.Now.AddDays(365)</code>
        /// </summary>
        /// <param name="frequency">how often to backup files</param>
        public BackupSchedule(BackupFrequency frequency)
            : this(frequency, DateTime.Now) { }

        /// <summary>
        /// Create a new BackupFrequency with specified frequency and start date, and
        /// default end date.
        /// Defaults:
        /// <code>EndDate: DateTime.Now.AddDays(365)</code>
        /// </summary>
        /// <param name="frequency">how often to backup files</param>
        /// <param name="startDate">when to start backing up</param>
        public BackupSchedule(BackupFrequency frequency, DateTime startDate)
            : this(frequency, startDate, DateTime.Now.AddDays(365)) { }

        /// <summary>
        /// Create a new BackupFrequency with specified params
        /// </summary>
        /// <param name="frequency">how often to backup files</param>
        /// <param name="startDate">when to start backing up</param>
        /// <param name="endDate">when to stop backing up</param>
        public BackupSchedule(BackupFrequency frequency, DateTime startDate, DateTime endDate)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Frequency = frequency;
        }

        /// <summary>
        /// Overrides the ToString method to return human readable information about schedule.
        /// </summary>
        /// <returns>"every {Amount} {Frequency} starting on {StartDate}</returns>
        public override string ToString()
        {
            return "every " + Frequency.Amount + " " + Frequency.Period.ToString() + " starting on " + StartDate.ToString();
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
