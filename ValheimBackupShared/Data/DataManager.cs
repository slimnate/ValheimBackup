using System;
using System.Collections.Generic;
using System.IO;
using ValheimBackup.BO;
using ValheimBackup.FTP;
using ValheimBackupShared.Properties;

namespace ValheimBackup.Data
{
    /// <summary>
    /// Static class that provides methods for managing local world files
    /// </summary>
    public static class DataManager
    {
        /// <summary>
        /// Directory to store data for this application.
        /// </summary>
        public static string AppDataDirectory
        {
            get
            {
                return Settings.Default.AppDataDirectory;
            }
        }

        /// <summary>
        /// File path for serialized server objects.
        /// </summary>
        public static string ServersFilePath
        {
            get
            {
                return Path.Combine(AppDataDirectory, Settings.Default.ServersFileName);
            }
        }

        /// <summary>
        /// File path for serialized Backup objects.
        /// </summary>
        public static string BackupsFilePath
        {
            get
            {
                return Path.Combine(AppDataDirectory, Settings.Default.BackupsFileName);
            }
        }

        /// <summary>
        /// Returns the default app data directory, computed as:
        /// "%appdata%/Common/ValheimBackup"
        /// </summary>
        private static string DefaultAppDataDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ValheimBackup");
            }
        }

        /// <summary>
        /// Initialize default settings. Should be called by the consumer
        /// before using any other DataManager functionality.
        /// </summary>
        public static void InitializeSettings()
        {
            if (Settings.Default.AppDataDirectory == "" || Settings.Default.DefaultBackupDirectory == "")
            {
                //default settings if they aren't setup
                Settings.Default.AppDataDirectory = DefaultAppDataDirectory;
                Settings.Default.DefaultBackupDirectory = Path.Combine(DefaultAppDataDirectory, "backups");

                //save
                SaveSettings();
            }
        }

        /// <summary>
        /// Saves the current application settings to disk.
        /// </summary>
        public static void SaveSettings()
        {
            //save user settings
            //https://docs.microsoft.com/en-us/visualstudio/ide/managing-application-settings-dotnet?view=vs-2019
            Settings.Default.Save();
        }

        /// <summary>
        /// Create a file and it's directory path if any don't exist,
        /// and then return a Stream to the newly created file.
        /// <b>Caller should be sure to close the stream after use!</b>
        /// </summary>
        /// <param name="path">file path to create</param>
        /// <returns>FileStream for the newly created file</returns>
        public static FileStream CreateAndOpenFile(string path)
        {
            string directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);
            return File.Create(path);
        }

        /// <summary>
        /// Backup a list of ftp files for a specific server.
        /// <br/><br/>
        /// Loads in the existing backups, uses a BackupBuilder instance
        /// to create all the new backup objects, file pairs, and write the
        /// backup files to disk. Merges new backups with existing ones.
        /// Finally clean up any old backups that should be deleted, and
        /// return the final updated list to the caller.
        /// </summary>
        /// <param name="server">Server the files are associated with</param>
        /// <param name="files">FtpFileInfo list to backup.</param>
        /// <returns>Updated list of Backups</returns>
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

        /// <summary>
        /// Write the contents of a remote Ftp file to the specified location
        /// on disk.
        /// </summary>
        /// <param name="file">file to write</param>
        /// <param name="destination">destination path</param>
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

        /// <summary>
        /// Removes all fiels from disk that are associated with a specific
        /// backup instance.
        /// </summary>
        /// <param name="backup">Backup to delete all files for.</param>
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
