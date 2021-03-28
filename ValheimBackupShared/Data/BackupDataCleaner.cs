using System;
using System.Collections.Generic;
using System.Linq;
using ValheimBackup.BO;
using ValheimBackup.Utils;

namespace ValheimBackup.Data
{
    /// <summary>
    /// This class takes care of cleaning up old backup files for a
    /// specific server
    /// </summary>
    public class BackupDataCleaner
    {
        private long serverId;
        private CleanupFrequency frequency;
        private List<Backup> backups;

        /// <summary>
        /// Create a new BackupDataCleaner with specified params
        /// </summary>
        /// <param name="server">The server that is currently being processed.</param>
        /// <param name="backups">The list of backups that currently exist for that server.</param>
        public BackupDataCleaner(Server server, List<Backup> backups)
        {
            this.serverId = server.Id;
            this.frequency = server.BackupSettings.CleanupSchedule;
            this.backups = backups;
        }

        /// <summary>
        /// The method that will be used by the consumer of this class
        /// to perform the cleanup process. Determines what cleanup settings
        /// the user has selected, and performs the correct cleanup accordingly
        /// </summary>
        /// <returns>List of Backups, less those that were removed.</returns>
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

        /// <summary>
        /// Internal method that will clean up backups based on the number of copies per world.
        /// <br/><br/>
        /// Filters the backups to only this servers, then groups by world name.
        /// Based on the number of copies found, and the specified copy limit,
        /// determine how many backups need to be deleted, and delete accordingly
        /// </summary>
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

        /// <summary>
        /// Internal method that will clean up files by their age.
        /// <br/><br/>
        /// Filters the files for only this server, and then determines
        /// the max age of any file for this server based on the current
        /// time and the cleanup frequency specified. Delete all backups
        /// that are older than this.
        /// </summary>
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

        /// <summary>
        /// Deletes files for a specified backup and removes it from the
        /// internal list of backups.
        /// <br/><br/>
        /// Uses the <code>DataManager.DeleteFilesFor(backup)</code> method
        /// to remove the files from disk.
        /// </summary>
        /// <param name="toRemove">Backup to remove</param>
        private void Remove(Backup toRemove)
        {
            DataManager.DeleteFilesFor(toRemove);
            backups.Remove(toRemove);
        }

        /// <summary>
        /// Filters a list of Backups by the server Id of each backup.
        /// Matches based on the internal server id property.
        /// </summary>
        /// <param name="backups">List to filter</param>
        /// <returns>List of filtered backups</returns>
        private IEnumerable<Backup> FilterByServer(IEnumerable<Backup> backups)
        {
            return backups.Where(b => b.ServerId == serverId);
        }

        /// <summary>
        /// Groups each backup in a list to their own list based on world name.
        /// </summary>
        /// <param name="backups">List of backups to group</param>
        /// <returns>Dictionary of lists, with key being the world name.</returns>
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

        /// <summary>
        /// Finds the oldest backup in the specified list, based on the
        /// <code>BackupTime</code> property of each.
        /// </summary>
        /// <param name="backups">List of backups to search</param>
        /// <returns>The oldest backup found in the list.</returns>
        private Backup GetOldest(IEnumerable<Backup> backups)
        {
            Backup res = null;
            foreach(var current in backups)
            {
                if (res == null)
                {
                    //first time through loop, initialize res so we have something to compare to
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
