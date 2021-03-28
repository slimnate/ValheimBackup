using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using ValheimBackup.BO;
using ValheimBackupShared.Properties;

namespace ValheimBackup.Data
{
    /// <summary>
    /// Static class which provides methods for managing Server data (the JSON list of saved servers)
    /// </summary>
    public class ServerDataManager
    {
        /// <summary>
        /// File path to the servers file, computed from application settings
        /// </summary>
        private static string ServersFilePath
        {
            get
            {
                return Path.Combine(Settings.Default.AppDataDirectory, Settings.Default.ServersFileName);
            }
        }


        /// <summary>
        /// Creates the servers file if it does not yet exist on disk.
        /// </summary>
        private static void createFileIfNotExist()
        {
            if (!Directory.Exists(Settings.Default.AppDataDirectory))
            {
                Directory.CreateDirectory(Settings.Default.AppDataDirectory);
            }

            if (!File.Exists(ServersFilePath))
            {
                using(File.Create(ServersFilePath))
                {
                    // empty using block to ensure stream closes
                }
            }
        }

        /// <summary>
        /// Deserialize the list of servers from the local disk.
        /// </summary>
        /// <returns>List of server objects that were read from the file.</returns>
        public static List<Server> LoadData()
        {
            try
            {
                var serialized = File.ReadAllText(ServersFilePath);
                var servers = JsonConvert.DeserializeObject<List<Server>>(serialized);

                if (servers == null) servers = new List<Server>();

                return servers;
            }
            catch(FileNotFoundException e)
            {
                Log("LoadData", "servers.json file does not exist, returning empty server list");
                //return empty list if file not exists or any other exception.
                return new List<Server>();
            }
            catch(Exception e)
            {
                Log("LoadData", "Error loading data from " + ServersFilePath);
                Log("LoadData", e.GetType().FullName + " - " + e.Message);

                //return empty list if file not exists or any other exception.
                return new List<Server>();
            }
        }

        /// <summary>
        /// Serializes the list of local servers to the disk.<br />
        /// <b>Does NOT append to existing file, but overwrites entirely!</b>
        /// Make sure that you supply the entire list of servers to this method,
        /// not just a subset to append.
        /// </summary>
        /// <param name="servers">List of backups to be saved</param>
        public static void SaveData(List<Server> servers)
        {
            try
            {
                //make sure file and folder exists
                createFileIfNotExist();

                var serialized = JsonConvert.SerializeObject(servers);

                File.WriteAllText(ServersFilePath, serialized);
            } catch(Exception e)
            {
                Log("LoadData", "Error saving data to " + ServersFilePath);
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
            Console.WriteLine("[ServerDataManager." + methodName + "]: " + message);
        }
    }
}
