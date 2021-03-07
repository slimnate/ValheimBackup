using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using ValheimBackup.BO;

namespace ValheimBackup.FTP
{
    public class FtpManager
    {
        private const string LIST = WebRequestMethods.Ftp.ListDirectoryDetails;
        private const string DOWNLOAD = WebRequestMethods.Ftp.DownloadFile;
        private const string UPLOAD = WebRequestMethods.Ftp.UploadFile;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="path"></param>
        /// <param name="method"></param>
        /// <returns>FtpWebRequest - request object for specified options.</returns>
        /// <exception cref="FtpRequestException">There was a problem with creating or configuring the request.</exception>
        private static FtpWebRequest Request(FtpConnectionInfo conn, string path, string method, int timeout=5000)
        {
            var uri = "ftp://" + conn.FullUri + path;
            var user = conn.Username;
            var pass = conn.Password;

            FtpWebRequest request = null;
            try
            {
                request = WebRequest.Create(uri) as FtpWebRequest;
                request.Method = method;
                request.Credentials = new NetworkCredential(user, pass);
                request.KeepAlive = false;
                request.Timeout = timeout;

                //not needed, as they already default to true
                //request.UseBinary = true;
                //request.UsePassive = true;
            }
            catch (SecurityException e)
            {
                throw new FtpRequestException("Request security exception: " + e.Message, request, e);
            } catch(FormatException e)
            {
                throw new FtpRequestException("Request format exception: " + e.Message, request, e);
            } catch(Exception e)
            {
                throw new FtpRequestException("Request failed: " + e.Message, request, e);
            }

            return request;
        }

        /// <summary>
        /// Try a getting a response and wraps exceptions with abstracted custom types.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static FtpWebResponse TryResponse(FtpWebRequest request)
        {
            FtpWebResponse response = null;
            try
            {
                response = request.GetResponse() as FtpWebResponse;
            }
            catch (WebException e)
            {
                //   T:System.Net.WebException:
                //     System.Net.FtpWebRequest.EnableSsl is set to true, but the server does not support
                //     this feature.- or -A System.Net.FtpWebRequest.Timeout was specified and the timeout
                //     has expired.
                throw new FtpResponseException("Can't get response - " + e.Message, response, e);
            }
            catch (InvalidOperationException e)
            {
                //   T:System.InvalidOperationException:
                //     System.Net.FtpWebRequest.GetResponse or System.Net.FtpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)
                //     has already been called for this instance.- or -An HTTP proxy is enabled, and
                //     you attempted to use an FTP command other than System.Net.WebRequestMethods.Ftp.DownloadFile,
                //     System.Net.WebRequestMethods.Ftp.ListDirectory, or System.Net.WebRequestMethods.Ftp.ListDirectoryDetails.
                throw new FtpResponseException("Can't get response - " + e.Message, response, e);
            }
            return response;
        }

        /// <summary>
        /// Performs a test of the FTP connection details provided, returning the
        /// server welcome message if succesful, otherwise throwing an exception.
        /// </summary>
        /// <param name="conn">FtpConnectionInfo - connection info to test</param>
        /// <returns>Server welcome message</returns>
        /// <exception cref="FtpRequestException">Problem with request.</exception>
        /// <exception cref="FtpResponseException">Problem with response</exception>
        public static string Test(FtpConnectionInfo conn)
        {
            var request = Request(conn, "/", LIST);
            var response = TryResponse(request);

            if(response.StatusCode == FtpStatusCode.OpeningData)
            {
                return response.WelcomeMessage;
            }

            throw new FtpResponseException("Unexpected response: ", response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="worldPath"></param>
        /// <returns></returns>
        public static List<FtpFileInfo> ListFiles(FtpConnectionInfo conn, string worldPath)
        {
            try
            {
                //clever trick to remove leading/trailing slashes on the world path. slashes will be added automatically
                worldPath = String.Join("/", worldPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
            } catch (ArgumentException e)
            {
                throw new FtpException("Invalid world path: " + worldPath, e);
            }

            var request = Request(conn, "/" + worldPath, LIST);
            var response = TryResponse(request);

            if (response.StatusCode == FtpStatusCode.OpeningData)
            {
                var files = new List<FtpFileInfo>();

                using (Stream responseStream = response.GetResponseStream()) //usings to ensure stream cleanup
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        while(!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            try
                            {
                                var ftpInfo = new FtpFileInfo(line);
                                files.Add(ftpInfo);
                            }
                            catch (FtpFileStringFormatException e)
                            {
                                Console.WriteLine("[Ftp.ListFiles] - error parsing file info");
                                Console.WriteLine("[Ftp.ListFiles] - " + e);
                            }
                            Console.WriteLine(line);
                        }

                        return files;
                    }
                }
            }

            //throw exception if we don't get the 150 Opening data status code
            throw new Exception("not connected");
        }

        public static List<FtpFileInfo> DownloadWorldFiles(Server server)
        {
            //variable definitions
            var conn = server.ConnectionInfo;
            var settings = server.BackupSettings;
            string worldPath = settings.WorldDirectory;


            //first get a list of all files in world path
            var files = ListFiles(conn, worldPath);

            //filter only files that should be backed up.
            files = new List<FtpFileInfo>(files.Where(f =>  settings.ShouldBackup(f.Name, f.Extension)));

            foreach(FtpFileInfo f in files)
            {
                try
                {
                    var path = Path.Combine(worldPath, f.FullName);
                    var contents = DownloadFile(conn, path);

                    f.Contents = contents;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error downloading file contents: ", e);
                }
            }

            return files;
        }

        public static string DownloadFile(FtpConnectionInfo conn, string path)
        {
            var request = Request(conn, path, DOWNLOAD);
            var response = TryResponse(request);

            if(response.StatusCode == FtpStatusCode.OpeningData)
            {
                try
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new FtpResponseException("error reading response stream while downloading file: " + path, e);
                }
            }

            throw new FtpResponseException("Unable to download file (incorrect status code): " + path);
        }

    }

    public class FtpException : Exception
    {
        public FtpException() : base() { }

        public FtpException(string message) : base(message) { }

        public FtpException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class FtpRequestException : FtpException
    {
        public FtpWebRequest Request { get; set; }

        public FtpRequestException() : base() { }

        public FtpRequestException(string message) : base(message) { }

        public FtpRequestException(string message, Exception innerException) : base(message, innerException) { }

        public FtpRequestException(string message, FtpWebRequest request, Exception innerException)
            : base(message, innerException)
        {
            Request = request;
        }

        public override string ToString()
        {
            return "FtpRequestException: " + Message;
        }
    }

    public class FtpResponseException : FtpException
    {
        public FtpWebResponse Response { get; set; }

        public FtpResponseException() : base() { }

        public FtpResponseException(string message) : base(message) { }

        public FtpResponseException(string message, Exception innerException) : base(message, innerException) { }

        public FtpResponseException(string message, FtpWebResponse response) : base(message)
        {
            Response = response;
        }

        public FtpResponseException(string message, FtpWebResponse response, Exception innerException)
            : base(message, innerException)
        {
            Response = response;
        }

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
