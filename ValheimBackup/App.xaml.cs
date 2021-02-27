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
using ValheimBackup.Properties;

namespace ValheimBackup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static BetterObservableCollection<Server> Servers { get; set; }
        public static BetterObservableCollection<Backup> Backups { get; set; }

        private static string DefaultAppDataDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ValheimBackup");
            }
        }

        public App() { }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //init application settings
            InitializeSettings();

            //load in server data if it exits
            InitializeAppData();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //save data before exiting
            PersistAppData();
        }

        private void InitializeSettings()
        {
            if(Settings.Default.AppDataDirectory == "" || Settings.Default.DefaultBackupDirectory == "")
            {
                //default settings if they aren't setup
                Settings.Default.AppDataDirectory = DefaultAppDataDirectory;
                Settings.Default.DefaultBackupDirectory = Path.Combine(DefaultAppDataDirectory, "backups");
                Settings.Default.Save();
            }
        }

        private void InitializeAppData()
        {
            //deserialize application data
            Servers = new BetterObservableCollection<Server>(ServerDataManager.LoadData());

            //add event listeners to save each time server collection changes.
            Servers.CollectionChanged += Servers_Changed;
        }

        private void Servers_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PersistAppData();
        }

        public void PersistAppData()
        {
            //serialize application data
            ServerDataManager.SaveData(Servers.ToList());

           // BackupDataManager.SaveData(Backups);

            //save user settings
            //https://docs.microsoft.com/en-us/visualstudio/ide/managing-application-settings-dotnet?view=vs-2019
            Settings.Default.Save();
        }
    }
}
