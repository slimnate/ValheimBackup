using System;
using System.Net;

namespace ValheimBackup.FTP
{
    /// <summary>
    /// Represents an exception with an FTP response, with the response object
    /// that cause the exception where applicable.
    /// </summary>
    public class FtpResponseException : FtpException
    {
        /// <summary>
        /// Response object that caused the exception.
        /// </summary>
        public FtpWebResponse Response { get; set; }

        /// <summary>
        /// Create new exception with no details.
        /// </summary>
        public FtpResponseException() : base() { }

        /// <summary>
        /// Create new exception with message only.
        /// </summary>
        /// <param name="message">description of the exception</param>
        public FtpResponseException(string message) : base(message) { }

        /// <summary>
        /// Create new exception with message and inner exception.
        /// </summary>
        /// <param name="message">description of the exception</param>
        /// <param name="innerException">inner exception that caused this one</param>
        public FtpResponseException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Create new exception and include the response that caused the
        /// exception to be thrown.
        /// </summary>
        /// <param name="message">description of the exception</param>
        /// <param name="response">the response that caused the exception</param>
        public FtpResponseException(string message, FtpWebResponse response) : base(message)
        {
            Response = response;
        }

        /// <summary>
        /// Create new exception and include the response that caused the
        /// exception to be thrown.
        /// </summary>
        /// <param name="message">description of the exception</param>
        /// <param name="response">the response that caused the exception</param>
        /// <param name="innerException">inner exception that caused this one</param>
        public FtpResponseException(string message, FtpWebResponse response, Exception innerException)
            : base(message, innerException)
        {
            Response = response;
        }

        /// <summary>
        /// Override default ToString() method
        /// </summary>
        /// <returns>
        /// String in the format: <code>"FtpResponseException: {Message}"</code>
        /// <br /><br />
        /// If this exception has a <see cref="Response"/> property that is not
        /// null, includes additional line(s) in the format: <br />
        /// <code>"\r\nResponse Status: {Response.StatusCode}\r\nStatus
        /// Description: {Response.StatusDescription}"</code>
        /// </returns>
        public override string ToString()
        {
            string res = "FtpResponseException: " + Message;

            if(Response != null)
            {
                res += "\r\nResponse Status: " + Response.StatusCode + "\r\nStatus Description:" + Response.StatusDescription;
            }

            return res;
        }
    }
}
