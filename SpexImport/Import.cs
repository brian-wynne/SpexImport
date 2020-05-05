//This program is for importing Etilize Data in to a MySQL Database.
//It will download the necessary files from FTP, extract them, and import the .csv files to the Database
//@Author: Brian Wynne

using System;
using System.Threading;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using IniParser;
using IniParser.Model;
using System.Runtime.InteropServices;
using System.Net.Sockets;

//Obtain .dll from C++, import to C#
//This will prevent the computer from sleeping
internal static class NativeMethods
{
    // Import SetThreadExecutionState Win32 API and necessary flags
    [DllImport("kernel32.dll")]
    public static extern uint SetThreadExecutionState(uint esFlags);
    public const uint ES_CONTINUOUS = 0x80000000;
    public const uint ES_SYSTEM_REQUIRED = 0x00000001;
}

namespace SpexImport
{
    class Import
    {
        private static string db_host, db_user, db_pass, db_name;
        private static uint db_port;

        private static string ftp_user, ftp_pass, ftp_url, ftp_files;
        private static readonly string BUILD_VERSION = "1.04";

        static void Main(string[] args)
        {
            Console.Title = "Spex Importer: V" + BUILD_VERSION;
            NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED);

            DeleteSpexDirectory(); //This method is for cleanup at the end, but if there were any issues it will the cleanup at the beginning
            LoadConfiguration();

            //TODO: Add this to loop from string of the ini file

            DownloadFromFTP("ftp://ftp.etilize.com/IT_CE/content/EN_US/basic/basic_EN_US_current_mysql.zip", "basic.zip");
            DownloadFromFTP("ftp://ftp.etilize.com/IT_CE/content/EN_US/accessories/accessories_EN_US_current_mysql.zip", "accessories.zip");
            DownloadFromFTP("ftp://ftp.etilize.com/IT_CE/tax/EN_US/tax_EN_US_current_mysql.zip", "tax.zip");

            UnzipCatalogContents("basic.zip");
            UnzipCatalogContents("accessories.zip");
            UnzipCatalogContents("tax.zip");

            ConnectToDatabase();

            System.Environment.Exit(0);
        }

        static void LoadConfiguration()
        {
            var parser = new FileIniDataParser();
            string file = @".\..\speximport.ini";
            if (!File.Exists(file)) //File not found, write new INI file with defaults
            {
                IniData info = new IniData();

                info["Database"]["host"] = "localhost";
                info["Database"]["user"] = "root";
                info["Database"]["pass"] = "";
                info["Database"]["db"] = "";
                info["Database"]["port"] = "3306";

                info["FTPCredentials"]["user"] = "";
                info["FTPCredentials"]["pass"] = "";
                info["FTPCredentials"]["dir"] = "";
                info["FTPCredentials"]["files"] = "";

                parser.WriteFile(file, info);
                Console.WriteLine("[INI] speximport.ini was not found, created new file. Please change from default values before running this...");
                Thread.Sleep(5 * 1000); //Hang the error message for 5 seconds so it can be read
                System.Environment.Exit(0);
                return;
            }

            IniData data = parser.ReadFile(file);
 
            db_host = data["Database"]["host"];
            db_user = data["Database"]["user"];
            db_pass = data["Database"]["pass"];
            db_name = data["Database"]["db"];

            db_port = Convert.ToUInt32(data["Database"]["port"]);

            ftp_user = data["FTPCredentials"]["user"];
            ftp_pass = data["FTPCredentials"]["pass"];
            ftp_url  = data["FTPCredentials"]["url"];
            ftp_files = data["FTPCredentials"]["files"];

            Logger("[INI] Configuration has been loaded: \n");
            //Console.WriteLine(data + "\n");
        }

        static void DownloadFromFTP(string url, string filename)
        {
            Logger("[FTP] Downloading " + filename + "...");

            FtpWebRequest request;

            try
            {
                request = (FtpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential(ftp_user, ftp_pass);
                request.UseBinary = true;
                request.UsePassive = true;
                request.Proxy = null;
                request.KeepAlive = true;
                request.Method = WebRequestMethods.Ftp.DownloadFile;
            }
            catch (Exception ex)
            {
                Logger("[FTP] Was unable to connect to '" + url + "'\n");
                Logger(ex + "\n");
                return;
            }
            //FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            FtpWebResponse response;

            try
            {
                response = (FtpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                Logger("[FTP] An unexpected error has occured.");
                return;
            }

            Stream responseStream = response.GetResponseStream();
            FileStream file = File.Create(filename);

            byte[] buffer = new byte[32 * 1024];
            int read;
            int total = 0;

            Console.Write("\n");

            try
            {
                while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    file.Write(buffer, 0, read);

                    total += read;
                    //var progress = (total) / (totalbytes);
                    Console.Write("\r{0} MB   ", (total/1000000));
                }
            }
            catch (WebException)
            {
                Logger("\n[FTP] An error occured... ");
                //Logger(ex + "\n");
            }

            file.Close();
            response.Close();

            Logger(" Done\n");
        }

        static void UnzipCatalogContents(string file)
        {
            Logger("[FTP] Extracting " + file + "...");

            string zipPath = @".\" + file;
            string extractPath = @".\spex";

            if (File.Exists(zipPath))
            {
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                File.Delete(Directory.GetCurrentDirectory() + @"\" + file);

                Logger(" Done\n");
                return;
            }

            Logger("[FTP] Unable to extract " + file + "\n");
            Logger(" Done\n");
        }

        static void ConnectToDatabase()
        {
            var tables = new Dictionary<string, string>()
            {
                //basic.zip
                { "EN_US_B_product.csv", "product" },
                { "EN_US_B_productattributes.csv", "product_attributes" },
                { "EN_US_B_productdescriptions.csv", "product_descriptions" },
                { "EN_US_B_productfeaturebullets.csv", "product_featurebullets" },
                { "EN_US_B_productlocales.csv", "product_locales" },
                { "EN_US_A_productaccessories.csv", "product_accessories" },
                { "EN_US_B_searchattributes.csv", "search_attributes" },
                { "EN_US_B_productkeywords.csv", "product_keywords" },

                //tax.zip
                { "EN_US_attributenames.csv", "attributenames" },
                { "EN_US_categorynames.csv", "categorynames" },
                { "EN_US_headernames.csv", "headernames" },
                { "EN_US_locales.csv", "locales" },
                { "EN_US_unitnames.csv", "unitnames" }
            };

            MySqlConnectionStringBuilder connStr = new MySqlConnectionStringBuilder();
            connStr.Server = db_host;
            connStr.UserID = db_user;
            connStr.Password = db_pass;
            connStr.Database = db_name;
            connStr.Port = db_port;
            connStr.AllowLoadLocalInfile = true;

            MySqlConnection conn = new MySqlConnection(connStr.ToString());

            try
            {
                Logger("[MySQL] Establishing connection to MySQL\n");
                conn.Open();
            }
            catch (Exception)
            {
                Logger("[MySQL] Connection was NOT established to MySQL\n");
                Logger("Program aborted...\n");
                Thread.Sleep(5 * 1000); //Show error message for 5 seconds before exiting
                System.Environment.Exit(0);
                return;
            }
            finally
            {
                //Load init.sql
                string script = File.ReadAllText(@".\..\init.sql");

                MySqlScript sqlScript = new MySqlScript(conn, script);
                sqlScript.Execute();

                ParseSpexDirectory(tables, conn);

                conn.Close();
                Logger("[MySQL] Connection closed\n");
            }

            DeleteSpexDirectory();
        }

        static void ParseSpexDirectory(dynamic tables, MySqlConnection conn)
        {
            string dir = Directory.GetCurrentDirectory();

            if (!Directory.Exists(dir + @"\spex"))
            {
                Logger("No directory found for 'spex' folder, aborting");
                return;
            }

            string[] files = Directory.GetFiles(dir + @"\spex\", "*.csv");

            for (int i = 0; i < files.Length; i++)
            {
                string file = Path.GetFileName(files[i]);
                foreach (var sqldata in tables)
                {
                    if (sqldata.Key == file)
                    {
                        Logger("[MySQL] Importing " + file + "...");
                        ImportSpexData(conn, sqldata.Key, sqldata.Value);
                        Logger(" Done\n");
                        break;
                    }
                }
            }
        }

        static void ImportSpexData(MySqlConnection conn, string filename, string tablename)
        {
            string dir = Directory.GetCurrentDirectory();
  
            MySqlBulkLoader bulk = new MySqlBulkLoader(conn);
            bulk.TableName = tablename;
            bulk.FieldTerminator = ",";
            bulk.FieldQuotationOptional = true;
            bulk.FieldQuotationCharacter = '"';
            bulk.EscapeCharacter = '\\';
            bulk.LineTerminator = "\r\n";
            bulk.CharacterSet = "LATIN1";
            bulk.FileName = dir + @"\spex\" + filename;
            bulk.Local = true;
            bulk.NumberOfLinesToSkip = 1;
            bulk.Load();
        }

        static void DeleteSpexDirectory()
        {
            string dir = Directory.GetCurrentDirectory();

            if (File.Exists(dir + @"\basic.zip"))
                File.Delete(dir + @"\basic.zip");

            if (File.Exists(dir + @"\accessories.zip"))
                File.Delete(dir + @"\accessories.zip");

            if (Directory.Exists(dir + @"\spex"))
            {
                string[] files = Directory.GetFiles(dir + @"\spex\", "*.csv");
                for (int i = 0; i < files.Length; i++)
                {
                    string file = Path.GetFileName(files[i]);
                    if (File.Exists(dir + @"\spex\" + file))
                        File.Delete(dir + @"\spex\" + file);
                }

                Directory.Delete(dir + @"\spex");
            }
        }

        static void Logger(string message, bool bprint=true)
        {
            string file = @".\..\log.txt";

            using (StreamWriter w = File.AppendText(file))
            {
                w.Write($"[{DateTime.Now.ToShortDateString()} @ {DateTime.Now.ToLongTimeString()}]");
                w.WriteLine(" " + message);
            }

            if (bprint)
                Console.Write(message);
        }
    }
}
