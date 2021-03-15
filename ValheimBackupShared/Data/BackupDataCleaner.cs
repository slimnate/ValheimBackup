using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValheimBackup.BO;
using ValheimBackup.Utils;

namespace ValheimBackup.Data
{
    public class BackupDataCleaner
    {
        private long serverId;
        private CleanupFrequency frequency;
        private List<Backup> backups;

        public BackupDataCleaner(Server server, List<Backup> backups)
        {
            this.serverId = server.Id;
            this.frequency = server.BackupSettings.CleanupSchedule;
            this.backups = backups;
        }

        public List<Backup> Clean()
        {
            //if copies, sort by world name and clean by copy count, otherwise clean by age.
            if (frequency.Period == CleanupPeriod.Copies)
            {
                CleanByWorldCopyCount();
            } else
            {
                CleanByAge();
            }

            return backups;
        }

        private void CleanByWorldCopyCount()
        {
            int maxCopies = frequency.Amount;
            var filtered = FilterByServer(backups);
            var groups = GroupByWorldName(filtered);

            foreach(var group in groups)
            {
                var list = group.Value as List<Backup>;

                // # of files to delete to ensure that the list has exactly 'maxCopies' items.
                // we do this calculation instead of just deleting the one oldest file, because
                // the user can perform manual backups from the UI, meaning we could need to
                //delete more than one
                int amountToDelete = list.Count() - maxCopies;

                while(amountToDelete > 0)
                {
                    //delete the oldest backup and remove from backup list
                    Remove(GetOldest(list));

                    //decrement counter
                    amountToDelete--;
                }
            }
        }

        private void CleanByAge()
        {
            var filtered = FilterByServer(backups);
            var maxAge = TimePeriod.TimeAgo(frequency);

            foreach(var backup in filtered)
            {
                if(DateTime.Compare(backup.BackupTime, maxAge) < 0)
                {
                    //backup is older than max age
                    Remove(backup);
                }
            }
        }

        private void Remove(Backup toRemove)
        {
            DataManager.DeleteFilesFor(toRemove);
            backups.Remove(toRemove);
        }

        private IEnumerable<Backup> FilterByServer(IEnumerable<Backup> backups)
        {
            return backups.Where(b => b.ServerId == serverId);
        }

        private Dictionary<string, IEnumerable<Backup>> GroupByWorldName(IEnumerable<Backup> backups)
        {
            var res = new Dictionary<string, IEnumerable<Backup>>();
            foreach(var backup in backups)
            {
                var worldName = backup.WorldName;
                if(!res.ContainsKey(worldName))
                {
                    //add key if not exists
                    res.Add(worldName, new List<Backup>());
                }
                //add backup to appropriate list
                ((List<Backup>)res[worldName]).Add(backup);
            }
            return res;
        }

        private Backup GetOldest(IEnumerable<Backup> backups)
        {
            Backup res = null;
            foreach(var current in backups)
            {
                if (res == null)
                {
                    res = current;
                }
                if(DateTime.Compare(current.BackupTime, res.BackupTime) < 0)
                {
                    //if backup time is earlier than res time, it's older.
                    res = current;
                }
            }
            return res;
        }
    }
}
