using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimBackup.BO
{
    public class Backup : INotifyPropertyChanged
    {
        private string _worldName;
        private string _sourcePath;
        private string _destinationPath;

        public string WorldName
        {
            get => _worldName;
            set
            {
                if (_worldName != value)
                {
                    _worldName = value;
                    NotifyPropertyChanged("WorldName");
                }
            }
        }
        public string SourcePath
        {
            get => _sourcePath;
            set
            {
                if (_sourcePath != value)
                {
                    _sourcePath = value;
                    NotifyPropertyChanged("SourcePath");
                }
            }
        }
        public string DestinationPath
        {
            get => _destinationPath;
            set
            {
                if (_destinationPath != value)
                {
                    _destinationPath = value;
                    NotifyPropertyChanged("DestinationPath");
                }
            }
        }

        public Backup()
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
    }
}
