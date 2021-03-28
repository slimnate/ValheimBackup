using System;

namespace ValheimBackup.FTP
{
    /// <summary>
    /// Represents an exception while parsing the file string for a
    /// FtpFileInfo object.
    /// </summary>
    public class FtpFileStringFormatException : FormatException
    {
        /// <summary>
        /// The file string that was being parsed when the exception occurred.
        /// </summary>
        public string FileString
        {
            get; set;
        }

        /// <summary>
        /// Create new FtpFileStringFormatException without an inner exception
        /// </summary>
        /// <param name="message">Description of the exception</param>
        /// <param name="fileString">file string that caused the error</param>
        public FtpFileStringFormatException(string message, string fileString) : base(message, null)
        {
            FileString = fileString;
        }

        /// <summary>
        /// Create new FtpFileStringFormatException that includes an inner exception
        /// </summary>
        /// <param name="message">Description of the exception</param>
        /// <param name="fileString">file string that caused the error</param>
        /// <param name="innerException">Exception to be wrapped by this one</param>
        public FtpFileStringFormatException(string message, string fileString, Exception innerException) : base(message, innerException)
        {
            FileString = fileString;
        }

        /// <summary>
        /// Return exception details as string in format of:
        /// {Message}: {FileString}
        /// </summary>
        /// <returns>String representation of exception</returns>
        public override string ToString()
        {
            return Message + ": " + FileString;
        }
    }
}
