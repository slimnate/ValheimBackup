using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValheimBackup.BO;
using ValheimBackup.FTP;
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

        public static List<Backup> BackupFtpFiles(Server server, List<FtpFileInfo> files)
        {
            //read in existing backup meta
            var backups = BackupDataManager.LoadData();

            foreach(FtpFileInfo file in files)
            {
                try
                {
                    //save file to disk
                    var backup = SaveFtpFile(file, server);
                    //add backup entry to list of existing backups
                    backups.Add(backup);
                } catch (Exception e)
                {
                    //TODO: Log this exception somewhere
                }
            }

            //cleanup backups
            var cleaner = new BackupDataCleaner(server, backups);

            return cleaner.Clean();
        }

        public static FileStream CreateAndOpenFile(string path)
        {
            string directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);
            return File.Create(path);
        }

        public static Backup SaveFtpFile(FtpFileInfo file, Server server)
        {
            var backup = new Backup(server, file);
            var backupFilePath = backup.DestinationPath;

            using (var stream = CreateAndOpenFile(backupFilePath))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(file.Contents);
                }
            }

            return backup;
        }

        public static void DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            } catch
            {
                throw; //TODO: implement exception handling
            }
        }
    }
}
