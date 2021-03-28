using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using ValheimBackup.BO;
using ValheimBackupShared.Properties;

namespace ValheimBackup.Data
{
    /// <summary>
    /// Static class which provides methods for managing Backup data (the JSON list of saved backups)
    /// </summary>
    public class BackupDataManager
    {
        /// <summary>
        /// File path for the backups file, computed from application settings.
        /// </summary>
        private static string BackupsFilePath
        {
            get
            {
                return Path.Combine(Settings.Default.AppDataDirectory, Settings.Default.BackupsFileName);
            }
        }

        /// <summary>
        /// Creates the backups file if it does not yet exist on disk.
        /// </summary>
        private static void createFileIfNotExist()
        {
            if (!Directory.Exists(Settings.Default.AppDataDirectory))
            {
                Directory.CreateDirectory(Settings.Default.AppDataDirectory);
            }

            if (!File.Exists(BackupsFilePath))
            {
                using (File.Create(BackupsFilePath))
                {
                    //empty using block to ensure stream closes
                }
            }
        }

        /// <summary>
        /// Deserialize the list of backups from the local disk.
        /// </summary>
        /// <returns>List of backup objects that were read from the file.</returns>
        public static List<Backup> LoadData()
        {
            try
            {
                var serialized = File.ReadAllText(BackupsFilePath);
                var backups = JsonConvert.DeserializeObject<List<Backup>>(serialized);

                if (backups == null) backups = new List<Backup>();

                return backups;
            }
            catch (FileNotFoundException e)
            {
                Log("LoadData", Settings.Default.BackupsFileName + " file does not exist, returning empty server list");
                //return empty list if file not exists or any other exception.
                return new List<Backup>();
            }
            catch (Exception e)
            {
                Log("LoadData", "Error loading data from " + BackupsFilePath);
                Log("LoadData", e.GetType().FullName + " - " + e.Message);

                //return empty list if file not exists or any other exception.
                return new List<Backup>();
            }
        }

        /// <summary>
        /// Serializes the list of local backups to the disk.<br />
        /// <b>Does NOT append to existing file, but overwrites entirely!</b>
        /// Make sure that you supply the entire list of backups to this method,
        /// not just a subset to append.
        /// </summary>
        /// <param name="backups">List of backups to be saved</param>
        public static void SaveData(List<Backup> backups)
        {
            try
            {
                //make sure file and folder exists
                createFileIfNotExist();

                var serialized = JsonConvert.SerializeObject(backups);

                File.WriteAllText(BackupsFilePath, serialized);
            }
            catch (Exception e)
            {
                Log("LoadData", "Error saving data to " + BackupsFilePath);
                Log("LoadData", e.GetType().FullName + " - " + e.Message);
            }
        }

        /// <summary>
        /// Logs a message to the console with some additional info about the message source.
        /// </summary>
        /// <param name="methodName">name of logging method</param>
        /// <param name="message">message to log</param>
        private static void Log(string methodName, string message)
        {
            Console.WriteLine("[BackupDataManager." + methodName + "]: " + message);
        }
    }
}
