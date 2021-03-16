using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace ValheimBackup.BO
{
    /// <summary>
    /// Represents a schedule for backing up world files.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BackupSchedule : INotifyPropertyChanged
    {
        private DateTime? _startDate;
        private DateTime? _endDate;


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

        [JsonProperty]
        public BackupFrequency Frequency { get; set; }

        public BackupSchedule() { }

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
}
