using Newtonsoft.Json;
using System.ComponentModel;

namespace ValheimBackup.BO
{
    /// <summary>
    /// Represents the frequency of occurance for cleanup events in the form of "every {amount} {unit}".
    /// eg. every 2 days, every 30 minutes, every 10 copies
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class CleanupFrequency : INotifyPropertyChanged
    {
        private int _amount;
        private CleanupPeriod _period;

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

        [JsonProperty]
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
