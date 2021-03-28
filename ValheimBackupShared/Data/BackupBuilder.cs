using System;
using System.Collections.Generic;
using System.IO;
using ValheimBackup.BO;
using ValheimBackup.FTP;

namespace ValheimBackup.Data
{
    /// <summary>
    /// BackupBuilder is a factory class that enables construction of a list of
    /// all the Backup objects from an individual batch of FTP files by
    /// providing individual BackupFilePair objects that will be associated
    /// with a Backup instance (which will be matched based on the world name,
    /// and created if it does not yet exist) - 
    /// This provides the benefit of making sure that all the files for a specific
    /// world will be associated with a single parent Backup instance, rather
    /// than having a separate instance for each file, and that only one backup
    /// instance will be created for a given world.
    /// <br/><br/>
    /// 
    /// Each of the ftp files will be saved to disk using the SaveFile delegate
    /// provided by the consumer of the factory as it is processed. This provides
    /// the benefit of being able to ensure each file is valid, and is acutally
    /// saved before it is added to the list of backups.
    /// <br/><br/>
    /// 
    /// This class also takes care of assigning a backup time to each backup as
    /// it is created, this ensures that each backup has a unique timestamp,
    /// which is vital as the id's and unique hashes are all derived from this
    /// </summary>
    public class BackupBuilder
    {
        private Server _server;
        private List<Backup> _backups;
        private DateTime _startTime;

        /// <summary>
        /// Delegate for a void method that can save a backup file to the disk
        /// </summary>
        /// <param name="info">FtpFileInfo instance to save</param>
        /// <param name="destination">Local path to write the file to</param>
        public delegate void SaveFile(FtpFileInfo info, string destination);

        /// <summary>
        /// Id of the Server that backups are being built for
        /// </summary>
        public long ServerId
        {
            get
            {
                return _server.Id;
            }
        }

        /// <summary>
        /// Computed property that returns the next unique backup time,
        /// ensuring that each backup object has it's own unique timestamp
        /// </summary>
        public DateTime NextBackupTime
        {
            get
            {
                _startTime = _startTime.AddMilliseconds(10);
                return _startTime;
            }
        }

        /// <summary>
        /// Create a new instance of BackupBuilder with specified params
        /// </summary>
        /// <param name="server"></param>
        public BackupBuilder(Server server)
        {
            _server = server;
            _backups = new List<Backup>();
            _startTime = DateTime.Now;
        }

        /// <summary>
        /// This method should be called once for each ftp file in the
        /// batch, and will take care of: <br />
        /// 1) Creating the file pair <br />
        /// 2) Adding the file pair to the correct backup, creating it
        /// if it does not exist. <br />
        /// 3) Saving the remote files contents to the local disk, using
        /// a supplied delgate method.
        /// </summary>
        /// <param name="file">FtpFileInfo instance to backup</param>
        /// <param name="save">Delegate method that will write a file to disk</param>
        public void AddFile(FtpFileInfo file, SaveFile save)
        {
            //find/create initial backup object
            var worldName = file.Name;
            var backup = GetBackup(worldName);

            //create backup file pair
            var filePair = GetFilePair(backup, file);

            //save file to disk using SaveFile delegate supplied by caller
            save(file, filePair.DestinationPath);

            //add pair to backup
            backup.AddFile(filePair);
        }

        /// <summary>
        /// Merges the backups created inside the builder with the backups
        /// that already exist in the supplied list.
        /// </summary>
        /// <param name="toMerge">List to merge internal backups into</param>
        /// <returns>The original <code>toMerge</code> list with new backups appended</returns>
        public List<Backup> MergeWith(List<Backup> toMerge)
        {
            //add each backup to toMerge
            foreach(var b in _backups)
            {
                toMerge.Add(b);
            }

            //return result
            return toMerge;
        }

        /// <summary>
        /// Returns the backup instance for a specific world name, creating
        /// one if it does not exist.
        /// </summary>
        /// <param name="worldName"></param>
        /// <returns></returns>
        private Backup GetBackup(string worldName)
        {
            Backup backup = null;

            //se4rch for matching backup
            foreach(var b in _backups)
            {
                if (b.WorldName == worldName) backup = b;
            }

            //create new one if not found
            if(backup == null)
            {
                backup = new Backup(ServerId, worldName, NextBackupTime);
                _backups.Add(backup);
            }

            return backup;
        }

        /// <summary>
        /// Creates a new instance of BackupFilePair for the specified backup
        /// and remote file combo, setting all relevant properties
        /// </summary>
        /// <param name="parent">the Backup object the file pair will be added to</param>
        /// <param name="file">The remote file to be added</param>
        /// <returns></returns>
        private BackupFilePair GetFilePair(Backup parent, FtpFileInfo file)
        {
            var fileName = file.Name;
            var timeHash = TimeHash(parent.BackupTime);
            var extension = file.Extension;

            var sourcePath = Path.Combine(_server.BackupSettings.WorldDirectory, file.FullName);
            var destinationPath = Path.Combine(_server.BackupSettings.BackupDirectory, fileName + "-" + timeHash + extension);

            return new BackupFilePair(sourcePath, destinationPath, fileName, timeHash, extension);
        }

        /// <summary>
        /// Returns a hased string representation of a specific DateTime object
        /// to the nearest 1/1000 of a second.
        /// </summary>
        /// <param name="dt">DateTime to hash</param>
        /// <returns>string representation of DateTime object in the form of: yyyyMMddHHmmssffff </returns>
        public static string TimeHash(DateTime dt)
        {
            return dt.ToString("yyyyMMddHHmmssffff");
        }
    }
}
