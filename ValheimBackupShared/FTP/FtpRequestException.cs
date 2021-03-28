using System;
using System.Net;

namespace ValheimBackup.FTP
{
    /// <summary>
    /// Represents an exception with creating an FTP request, with a copy of
    /// the offending request where applicable.
    /// </summary>
    public class FtpRequestException : FtpException
    {
        /// <summary>
        /// The request that caused the exception.
        /// </summary>
        public FtpWebRequest Request { get; set; }

        /// <summary>
        /// Create new exception with no details.
        /// </summary>
        public FtpRequestException() : base() { }

        /// <summary>
        /// Create new exception with message only.
        /// </summary>
        /// <param name="message">description of the exception</param>
        public FtpRequestException(string message) : base(message) { }

        /// <summary>
        /// Create new exception with message and inner exception.
        /// </summary>
        /// <param name="message">description of the exception</param>
        /// <param name="innerException">inner exception that caused this one</param>
        public FtpRequestException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Create new exception and include the request that caused the
        /// exception to be thrown.
        /// </summary>
        /// <param name="message">description of the exception</param>
        /// <param name="request">the request that caused the exception</param>
        /// <param name="innerException">inner exception that caused this one</param>
        public FtpRequestException(string message, FtpWebRequest request, Exception innerException)
            : base(message, innerException)
        {
            Request = request;
        }

        /// <summary>
        /// Override default ToString() method
        /// </summary>
        /// <returns>String in the format of: "FtpWebRequest: {Message}"</returns>
        public override string ToString()
        {
            return "FtpRequestException: " + Message;
        }
    }
}
