using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ValheimBackup.FTP
{
    public class FtpFileInfo
    {
        private const string DATE_FORMAT = "MMM dd HH:mm";
        private const string FILE_INFO_PATTERN =
            @"(?<perms>[drwx-]{10})\s\d+\sftp\sftp\s*(?<size>\d+)\s(?<modified>\w{1,3}\s\d{2}\s\d{2}:\d{2})\s(?<name>.*)";

        private string _name;
        private string _ext;
        private string _permissions;
        private string _contents;
        private int _size;
        private DateTime _lastModified;
        private bool _isDirectory;

        public string Name
        {
            get => _name;
        }

        public string Extension
        {
            get => _ext;
        }

        public string FullName
        {
            get => _name + _ext;
        }

        public string Permissions
        {
            get => _permissions;
        }

        public string Contents
        {
            get => _contents;
            set => _contents = value;
        }

        public DateTime LastModified
        {
            get => _lastModified;
        }

        public int Size
        {
            get => _size;
        }

        public double SizeKB
        {
            get => Size / 1024;
        }

        public double SizeMB
        {
            get => SizeKB / 1024;
        }

        public string SizeDisplay
        {
            get
            {
                if(SizeMB > 1)
                {
                    return Math.Round(SizeMB, 2, MidpointRounding.AwayFromZero) + " MB";
                }
                else if(SizeKB > 1)
                {
                    return Math.Round(SizeKB, 2, MidpointRounding.AwayFromZero) + " KB";
                }
                else
                {
                    return Size.ToString() + " bytes";
                }
            }
        }

        public bool IsDirectory
        {
            get => _isDirectory;
        }

        public bool IsFile
        {
            get => !_isDirectory;
        }

        public FtpFileInfo(string fileString)
        {
            if(!ValidFileString(fileString))
            {
                throw new FtpFileStringFormatException("Invalid file info string", fileString);
            }

            var matches = Regex.Match(fileString, FILE_INFO_PATTERN);

            _permissions = matches.Groups["perms"].Value;
            _size = int.Parse(matches.Groups["size"].Value);
            _lastModified = DateTime.ParseExact(matches.Groups["modified"].Value, DATE_FORMAT, CultureInfo.InvariantCulture);
            _isDirectory = _permissions[0] == 'd';

            var nameString = matches.Groups["name"].Value;
            if(nameString.Contains("."))
            {
                int dotIndex = nameString.IndexOf('.');
                _name = nameString.Substring(0, dotIndex);
                _ext = nameString.Substring(dotIndex);
                return;
            } else
            {
                _name = nameString;
                _ext = "";
            }
        }

        public static bool ValidFileString(string fileString)
        {
            try
            {
                var matches = Regex.Match(fileString, FILE_INFO_PATTERN);

                //valid file patterns should have 5 matches (including the 0-index match which is the entire string)
                return matches.Groups.Count == 5;
            } catch(Exception e)
            {
                throw new FtpFileStringFormatException("Unable to perform regex matching on file string", fileString, e);
            }
        }

        public override string ToString()
        {
            return (IsDirectory ? "[D]" : "[F]") + " " + FullName + " (" + LastModified.ToString(DATE_FORMAT) + " - " + SizeDisplay + ")";
        }
    }

    public class FtpFileStringFormatException : FormatException
    {
        public string FileString
        {
            get; set;
        }

        public FtpFileStringFormatException(string message, string fileString) : base(message, null)
        {
            FileString = fileString;
        }

        public FtpFileStringFormatException(string message, string fileString, Exception innerException) : base(message, innerException)
        {
            FileString = fileString;
        }

        public override string ToString()
        {
            return Message + ": " + FileString;
        }
    }
}
