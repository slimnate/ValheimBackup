using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValheimBackup.BO;
using ValheimBackup.Properties;

namespace ValheimBackup.Data
{
    public class ServerDataManager
    {
        private static string ServersFilePath
        {
            get
            {
                return Path.Combine(Settings.Default.AppDataDirectory, Settings.Default.ServersFileName);
            }
        }

        private static void createFileIfNotExist()
        {
            if (!Directory.Exists(Settings.Default.AppDataDirectory))
            {
                Directory.CreateDirectory(Settings.Default.AppDataDirectory);
            }

            if (!File.Exists(ServersFilePath))
            {
                using(File.Create(ServersFilePath)) {
                    //empy using block to ensure stream closes
                }
            }
        }

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

        private static void Log(string methodName, string message)
        {
            Console.WriteLine("[ServerDataManager." + methodName + "]: " + message);
        }
    }
}
