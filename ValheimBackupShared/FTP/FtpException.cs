using System;

namespace ValheimBackup.FTP
{
    /// <summary>
    /// Represents a generic FTP exception with optional message and inner
    /// exception.
    /// </summary>
    public class FtpException : Exception
    {
        public FtpException() : base() { }

        public FtpException(string message) : base(message) { }

        public FtpException(string message, Exception innerException) : base(message, innerException) { }
    }
}
