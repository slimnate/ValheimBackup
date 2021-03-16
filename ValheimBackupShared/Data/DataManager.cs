using System;
using System.Collections.Generic;
using System.IO;
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

        public static FileStream CreateAndOpenFile(string path)
        {
            string directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);
            return File.Create(path);
        }

        public static List<Backup> BackupFtpFiles(Server server, List<FtpFileInfo> files)
        {
            System.Diagnostics.Debugger.Launch();
            //read in existing backup meta
            var backups = BackupDataManager.LoadData();

            var builder = new BackupBuilder(server);

            foreach(FtpFileInfo file in files)
            {
                builder.AddFile(file, SaveFtpFile);
            }

            backups = builder.MergeWith(backups);

            //cleanup backups
            var cleaner = new BackupDataCleaner(server, backups);

            return cleaner.Clean();
        }

        public static void SaveFtpFile(FtpFileInfo file, string destination)
        {
            using (var stream = CreateAndOpenFile(destination))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(file.Contents);
                }
            }
        }

        public static void DeleteFilesFor(Backup backup)
        {
            foreach(BackupFilePair file in backup.Files)
            {
                try
                {
                    File.Delete(file.DestinationPath);
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
