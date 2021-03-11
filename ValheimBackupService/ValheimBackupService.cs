using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ValheimBackup.BO;
using ValheimBackup.Data;
using ValheimBackupShared.Properties;

namespace ValheimBackupService
{
    public partial class ValheimBackupService : ServiceBase
    {
        public List<ServerBackupTimer> backupTimers = new List<ServerBackupTimer>();
        public EventLog eventLog;

        public ValheimBackupService()
        {
            InitEventLog();
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();

            DataManager.InitializeSettings(); //init settings in case the service runs before the desktop app

            //load in current servers and backups
            List<Server> servers = ServerDataManager.LoadData();
            List<Backup> backups = BackupDataManager.LoadData();

            //Set up file watcher
            InitFileWatcher();

            //set up timers based on servers
            InitBackupTimers(servers);
        }

        private void InitEventLog()
        {
            eventLog = new EventLog("Application");
            eventLog.Source = "Valheim Backup Service";
        }

        private void InitFileWatcher()
        {
            //set up file watcher
            fileWatcher.Path = DataManager.AppDataDirectory;
            fileWatcher.NotifyFilter = System.IO.NotifyFilters.LastWrite; //set to only notify of last write
            fileWatcher.Filter = "*.json"; // only watch json files in this dir
            fileWatcher.Changed += OnFileChanged;
            //fileWatcher.Created += OnFileChange;
            //fileWatcher.Deleted += OnFileChange;
            fileWatcher.EnableRaisingEvents = true;
        }

        private void InitBackupTimers(List<Server> servers)
        {
            ClearAllTimers();

            foreach(Server s in servers)
            {
                var timer = new ServerBackupTimer(s);
                timer.Elapsed += OnTimerElapsed;
                timer.Enabled = true;

                backupTimers.Add(timer);
            }

            ModalMessage("Started " + backupTimers.Count + " timers");
        }

        private void ClearAllTimers()
        {
            foreach (ServerBackupTimer t in backupTimers)
            {
                t.Close();
                t.Dispose();
            }

            backupTimers = new List<ServerBackupTimer>();
        }

        private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Server server = ((ServerBackupTimer)sender).Server;
            var updateTime = e.SignalTime; //TODO: add last updated to backup upon completion

            ModalMessage("backing up server: " + server.NameAndDescription);


        }

        private void OnFileChanged(object sender, System.IO.FileSystemEventArgs e)
        {
            if(e.FullPath == DataManager.ServersFilePath)
            {
                //update servers
                ModalMessage("resettings backup timers");
                List<Server> servers = ServerDataManager.LoadData();
                InitBackupTimers(servers);
            }
            else if(e.FullPath == DataManager.BackupsFilePath)
            {
                //update backups? probably not necessary
                ModalMessage("updating backups?");
            }
        }

        protected override void OnStop()
        {
            fileWatcher.EnableRaisingEvents = false;
            ClearAllTimers();
        }

        private void ModalMessage(string s)
        {
            //also write these to event log in case problem with modal. Eventually modal will go to pop-up notifications, so log there as well
            eventLog.WriteEntry(s);

            MessageBox.Show(s, "ValheimBackup Service:", MessageBoxButtons.OK, MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }
    }
}
