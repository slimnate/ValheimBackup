using Newtonsoft.Json;
using System.ComponentModel;

namespace ValheimBackup.BO
{
    /// <summary>
    /// Represents a time period for backups
    /// 
    /// Implements INotifyPropertyChanged interface and Newtonsoft.JSON serialization attributes
    /// </summary>
    public enum BackupPeriod : int
    {
        Minutes, Hours, Days, Weeks
    }


    /// <summary>
    /// Represents the frequency of occurance for a backup event in the form of "every {amount} {unit}".
    /// eg. every 2 days, every 30 minutes, every 1 week
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BackupFrequency : INotifyPropertyChanged
    {
        private int _amount;
        private BackupPeriod _period;

        /// <summary>
        /// Frequency amount (number of units)
        /// </summary>
        [JsonProperty]
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

        /// <summary>
        /// Frequency period (units)
        /// </summary>
        [JsonProperty]
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

        /// <summary>
        /// Empty default constructor for JSON deserialization
        /// </summary>
        public BackupFrequency() { }

        /// <summary>
        /// Create new BackupFrequency from params
        /// </summary>
        /// <param name="amount">frequency amount</param>
        /// <param name="period">frequency period</param>
        public BackupFrequency(int amount, BackupPeriod period)
        {
            this.Amount = amount;
            this.Period = period;
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
