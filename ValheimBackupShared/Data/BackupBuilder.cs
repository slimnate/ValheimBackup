using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValheimBackup.BO;
using ValheimBackup.FTP;

namespace ValheimBackup.Data
{
    public class BackupBuilder
    {
        private Server _server;
        private List<Backup> _backups;
        private DateTime _startTime;

        public delegate void SaveFile(FtpFileInfo info, string destination);

        public long ServerId
        {
            get
            {
                return _server.Id;
            }
        }

        public DateTime NextBackupTime
        {
            get
            {
                _startTime = _startTime.AddMilliseconds(10);
                return _startTime;
            }
        }

        public BackupBuilder(Server server)
        {
            _server = server;
            _backups = new List<Backup>();
            _startTime = DateTime.Now;
        }

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

        private BackupFilePair GetFilePair(Backup parent, FtpFileInfo file)
        {
            var fileName = file.Name;
            var timeHash = TimeHash(parent.BackupTime);
            var extension = file.Extension;

            var sourcePath = Path.Combine(_server.BackupSettings.WorldDirectory, file.FullName);
            var destinationPath = Path.Combine(_server.BackupSettings.BackupDirectory, fileName + "-" + timeHash + extension);

            return new BackupFilePair(sourcePath, destinationPath, fileName, timeHash, extension);
        }

        public static string TimeHash(DateTime dt)
        {
            return dt.ToString("yyyyMMddHHmmssffff");
        }
    }
}
