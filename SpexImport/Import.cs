using System;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpexImport
{
    class Import
    {
        static void Main(string[] args)
        {
            DownloadFromFTP();
            System.Environment.Exit(0);
        }

        static void DownloadFromFTP()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://ftp.etilize.com/IT_CE/content/EN_US/basic/basic_EN_US_current_mysql.zip");
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            request.Credentials = new NetworkCredential("dhecsdelta", "8q@Hsb3lta");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            FileStream file = File.Create(@"basic.zip");
            byte[] buffer = new byte[32 * 1024];
            int read;
            
            while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                file.Write(buffer, 0, read);
            }

            Console.WriteLine($"Download complete, status {response.StatusDescription}");

            file.Close();
            response.Close();

            UnzipCatalogContents();
        }

        static void UnzipCatalogContents()
        {
            string zipPath = @".\basic.zip";
            string extractPath = @".\spex";

            ZipFile.ExtractToDirectory(zipPath, extractPath);
        }
    }
}
