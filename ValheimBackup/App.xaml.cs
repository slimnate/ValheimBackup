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

            //associate servers/backups with one another based on ID's
            AssociateCollections();

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

            //don't persist backup data until we have it updated with file watcher
            //BackupDataManager.SaveData(Backups.ToList());

            DataManager.SaveSettings();
        }

        /// <summary>
        /// Associates the server and backup collections with one another based on their ID's
        /// <br/>
        /// For <code>Server</code> objects, their <code>Backups</code> property will be populated
        /// with a <code>List<Backup></code> with all backups for this server.
        /// <br/>
        /// For <code>Backup</code> objects, their <code>Server</code> property will be populated with
        /// the <code>Server</code> that they came from.
        /// </summary>
        public static void AssociateCollections()
        {
            foreach (var server in Servers)
            {
                server.Backups = Backups.For(server) as List<Backup>;
            }

            foreach (var backup in Backups)
            {
                backup.Server = Servers.For(backup);
            }
        }
    }
}
