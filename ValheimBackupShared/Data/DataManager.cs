using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValheimBackupShared.Properties;

namespace ValheimBackup.Data
{
    public static class DataManager
    {
        public static string AppDataDirectory
        {
            get
            {
                return Settings.Default.AppDataDirectory;
            }
        }
        public static string ServersFilePath
        {
            get
            {
                return Path.Combine(AppDataDirectory, Settings.Default.ServersFileName);
            }
        }

        public static string BackupsFilePath
        {
            get
            {
                return Path.Combine(AppDataDirectory, Settings.Default.BackupsFileName);
            }
        }

        private static string DefaultAppDataDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ValheimBackup");
            }
        }

        public static void InitializeSettings()
        {
            if (Settings.Default.AppDataDirectory == "" || Settings.Default.DefaultBackupDirectory == "")
            {
                //default settings if they aren't setup
                Settings.Default.AppDataDirectory = DefaultAppDataDirectory;
                Settings.Default.DefaultBackupDirectory = Path.Combine(DefaultAppDataDirectory, "backups");
                Settings.Default.Save();
                return;
            }
        }

        public static void SaveSettings()
        {
            //save user settings
            //https://docs.microsoft.com/en-us/visualstudio/ide/managing-application-settings-dotnet?view=vs-2019
            Settings.Default.Save();
        }
    }
}
