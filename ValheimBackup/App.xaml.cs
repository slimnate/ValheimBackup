using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ValheimBackup.BO;
using ValheimBackup.Data;
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

        public App() { }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //init application settings
            DataManager.InitializeSettings();

            //load in server data if it exits
            InitializeAppData();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //save data before exiting
            PersistAppData();
        }

        private void InitializeAppData()
        {
            //deserialize application data
            Servers = new BetterObservableCollection<Server>(ServerDataManager.LoadData());
            Backups = new BetterObservableCollection<Backup>(BackupDataManager.LoadData());

            //add event listeners to save each time server collection changes.
            Servers.CollectionChanged += PersistantData_Changed;
            Backups.CollectionChanged += PersistantData_Changed;
        }

        private void PersistantData_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PersistAppData();
        }

        public void PersistAppData()
        {
            //serialize application data
            ServerDataManager.SaveData(Servers.ToList());

            BackupDataManager.SaveData(Backups.ToList());

            DataManager.SaveSettings();
        }
    }
}
