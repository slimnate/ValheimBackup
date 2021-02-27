using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ValheimBackup.BO;

namespace ValheimBackup.FTP
{
    public class FtpManager
    {
        public static bool Test(FtpConnectionInfo connectionInfo)
        {
            var uri = "ftp://" + connectionInfo.FullUri + "/";

            var user = connectionInfo.Username;
            var pass = connectionInfo.Password;

            FtpWebRequest request = WebRequest.Create(uri) as FtpWebRequest;

            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            request.Credentials = new NetworkCredential(user, pass);
            request.KeepAlive = false;
            request.UseBinary = true;
            request.UsePassive = true;

            FtpWebResponse response = request.GetResponse() as FtpWebResponse;

            if(response.StatusCode == FtpStatusCode.OpeningData)
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string val = reader.ReadToEnd();
                Console.WriteLine(val);
                return true;
            }
            
            return false;
        }
    }
}
