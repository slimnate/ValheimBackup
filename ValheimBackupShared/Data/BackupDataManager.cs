using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValheimBackup.BO;
using ValheimBackupShared.Properties;

namespace ValheimBackup.Data
{
    public class BackupDataManager
    {
        private static string BackupsFilePath
        {
            get
            {
                return Path.Combine(Settings.Default.AppDataDirectory, Settings.Default.BackupsFileName);
            }
        }

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

        private static void Log(string methodName, string message)
        {
            Console.WriteLine("[BackupDataManager." + methodName + "]: " + message);
        }
    }
}
