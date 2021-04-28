using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ValheimBackup.FTP
{
    /// <summary>
    /// Represents a file OR directory that has been downloaded from
    /// an external FTP server. 
    /// <br/><br/>
    /// Given a file string in the format returned by the FTP response, will
    /// parse the file string to determing information about the
    /// file/directory. Has the ability to store it's contents if the object
    /// is a file.
    /// </summary>
    public class FtpFileInfo
    {
        private const string DATE_TIME_FORMAT = "MMM dd HH:mm";
        private const string DATE_FORMAT = "MMM dd  yyyy";

        // regex editor: https://regex101.com/r/3Udbmn/1
        private const string FILE_INFO_PATTERN =
            @"(?<perms>[drwx-]{10})\s\d+\sftp\sftp\s*(?<size>\d+)\s(?<modified>\w{1,3}\s\d{2}\s*\d{2}:*\d{2})\s(?<name>.*)";

        private string _name;
        private string _ext;
        private string _permissions;
        private string _contents;
        private int _size;
        private DateTime _lastModified;
        private bool _isDirectory;

        /// <summary>
        /// Name of the remote file (excluding path and extension)
        /// </summary>
        public string Name
        {
            get => _name;
        }

        /// <summary>
        /// Extension of the remote file (including the preceding dot)
        /// </summary>
        public string Extension
        {
            get => _ext;
        }

        /// <summary>
        /// Full name of the remote file (including extension, excluding path)
        /// </summary>
        public string FullName
        {
            get => _name + _ext;
        }

        /// <summary>
        /// Unix-style permissions string representing the permissions of the
        /// file on the remote server.
        /// </summary>
        public string Permissions
        {
            get => _permissions;
        }

        /// <summary>
        /// Raw contents of the remote file. Used to write a copy to the local disk.
        /// </summary>
        public string Contents
        {
            get => _contents;
            set => _contents = value;
        }

        /// <summary>
        /// Date the remote file was last modified.
        /// </summary>
        public DateTime LastModified
        {
            get => _lastModified;
        }

        /// <summary>
        /// Size of the files contents in bytes, as represented in the remote
        /// file string.
        /// </summary>
        public int Size
        {
            get => _size;
        }

        /// <summary>
        /// File size converted to KB
        /// </summary>
        public double SizeKB
        {
            get => Size / 1024;
        }

        /// <summary>
        /// File size converted to MB
        /// </summary>
        public double SizeMB
        {
            get => SizeKB / 1024;
        }

        /// <summary>
        /// Returns a calculated string of the file size, using the smallest
        /// unit that would result in a size value greater than 1, and the
        /// appropriate unit label.
        /// </summary>
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

        /// <summary>
        /// True if this object represents a directory
        /// </summary>
        public bool IsDirectory
        {
            get => _isDirectory;
        }

        /// <summary>
        /// Ture if this object represents a file
        /// </summary>
        public bool IsFile
        {
            get => !_isDirectory;
        }

        /// <summary>
        /// Create a new FtpFileInfo object by parsing the details of the
        /// provided unix-style remote file string.
        /// </summary>
        /// <param name="fileString">the unix-style file description string to parse</param>
        public FtpFileInfo(string fileString)
        {
            if(!ValidFileString(fileString))
            {
                throw new FtpFileStringFormatException("Invalid file info string", fileString);
            }

            var matches = Regex.Match(fileString, FILE_INFO_PATTERN);

            _permissions = matches.Groups["perms"].Value;
            _size = int.Parse(matches.Groups["size"].Value);
            _isDirectory = _permissions[0] == 'd';

            var modifiedString = matches.Groups["modified"].Value;
            try
            {
                //try date format that includes time (newer files)
                _lastModified = DateTime.ParseExact(modifiedString, DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
            } catch(FormatException e)
            {
                try
                {
                    // fall back to date format with year in place of time (older files)
                    _lastModified = DateTime.ParseExact(modifiedString, DATE_FORMAT, CultureInfo.InvariantCulture);
                } catch { throw; }
            }

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

        /// <summary>
        /// Determines whether the provided file string is valid (determined
        /// by attempting to match the file-string regex, and detecting any
        /// errors). If the regex matches without any issues, the file string
        /// is considered valid.
        /// </summary>
        /// <param name="fileString">The file string to test for validity</param>
        /// <returns>
        /// If the string is valid: <code>true</code>
        /// Otherwise: <code>false</code>
        /// </returns>
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

        /// <summary>
        /// Returns a string representation of the details of the file, in the
        /// format of: <[D] / [F]> {FullName} ({LastModified} - {SizeDisplay} 
        /// </summary>
        /// <returns>String representation of object.</returns>
        public override string ToString()
        {
            return (IsDirectory ? "[D]" : "[F]") + " " + FullName + " (" + LastModified.ToString(DATE_TIME_FORMAT) + " - " + SizeDisplay + ")";
        }
    }
}
