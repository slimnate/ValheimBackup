using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ValheimBackup.BO;
using ValheimBackup.Extensions;

namespace ValheimBackup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static BetterObservableCollection<Server> Servers { get; set; }
        public static BetterObservableCollection<Backup> Backups { get; set; }

        public App()
        {

        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //load in data if it exits
            LoadData();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //save data before exiting
            SaveData();
        }

        public void LoadData()
        {
            //deserialize application data
        }

        public void SaveData()
        {
            //serialize application data
        }
    }
}
