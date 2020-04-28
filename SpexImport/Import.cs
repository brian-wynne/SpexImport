﻿//This program is for importing Etilize Data in to a MySQL Database.
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
using Microsoft.VisualBasic.Logging;

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
    class SQLTable
    {
        public string Filename { get; set; }
        public string QueryCmd { get; set; }
    }

    class Import
    {
        private static string db_host, db_user, db_pass, db_name;
        private static uint db_port;

        private static string ftp_user, ftp_pass;
        private static readonly string BUILD_VERSION = "1.0";

        static void Main(string[] args)
        {
            Console.Title = "Spex Importer: V" + BUILD_VERSION;
            NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED);

            DeleteSpexDirectory(); //This method is for cleanup at the end, but if there were any issues it will the cleanup at the beginning
            LoadConfiguration();

            Logger("[FTP] Downloading basic...");
            DownloadFromFTP("ftp://ftp.etilize.com/IT_CE/content/EN_US/basic/basic_EN_US_current_mysql.zip", "basic.zip");
            Logger(" Done\n");

            Logger("[FTP] Downloading accessories...");
            DownloadFromFTP("ftp://ftp.etilize.com/IT_CE/content/EN_US/accessories/accessories_EN_US_current_mysql.zip", "accessories.zip");
            Logger(" Done\n");

            Logger("[FTP] Extracting basic.zip...");
            UnzipCatalogContents("basic.zip");
            Logger(" Done\n");

            Logger("[FTP] Extracting accessories.zip...");
            UnzipCatalogContents("accessories.zip");
            Logger(" Done\n");

            ConnectToDatabase();
            //System.Environment.Exit(0);
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

                parser.WriteFile(file, info);
                Console.WriteLine("[ERROR] speximport.ini was not found, created new file. Please change from default values before running this...");
                Thread.Sleep(5 * 1000); //Hang the error message for 5 seconds so it can be read
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
        }

        static void DownloadFromFTP(string url, string filename)
        {
            FtpWebRequest request;

            try
            {
                request = (FtpWebRequest)WebRequest.Create(url);
            }
            catch (Exception ex)
            {
                Logger("[FTP] Was unable to connect to '" + url + "'\n");
                Logger(ex + "\n");
                return;
            }

            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential("dhecsdelta", "8q@Hsb3lta");
            request.UseBinary = true;
            //request.Proxy = null;
            //request.KeepAlive = true;

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            FileStream file = File.Create(filename);
            byte[] buffer = new byte[32 * 1024];
            int read;

            try
            {
                while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    file.Write(buffer, 0, read);
                }
            }
            catch (WebException ex)
            {
                Logger("[FTP] AN error occured while attempting to download " + filename + "\n");
                Logger(ex + "\n");
            }

            file.Close();
            response.Close();
        }

        static void UnzipCatalogContents(string folder)
        {
            string zipPath = @".\" + folder;
            string extractPath = @".\spex";

            ZipFile.ExtractToDirectory(zipPath, extractPath);
            File.Delete(Directory.GetCurrentDirectory() + @"\" + folder);
        }

        static void ConnectToDatabase()
        {
            var tables = new Dictionary<string, SQLTable>()
            {
                { "product", new SQLTable
                    {
                        Filename = "EN_US_B_product.csv",
                        QueryCmd = "CREATE TABLE IF NOT EXISTS product (productid INT NOT NULL, mfgid VARCHAR(16), mfgpn VARCHAR(128) NOT NULL, categoryid INT, is_active CHAR(1), equivalency TEXT, create_date TIMESTAMP, modify_date TIMESTAMP, last_update TIMESTAMP, PRIMARY KEY(productid, mfgpn));"
                    }
                },
                { "product_attributes", new SQLTable
                    {
                        Filename = "EN_US_B_productattributes.csv",
                        QueryCmd = "CREATE TABLE IF NOT EXISTS product_attributes (productid INT, attributeid BIGINT, setnumber SMALLINT, text TEXT, absolutevalue DOUBLE, unitid INT, isabsolute SMALLINT, isactive SMALLINT, localeid INT, type INT);"
                    }
                },
                { "product_descriptions", new SQLTable
                    {
                        Filename = "EN_US_B_productdescriptions.csv",
                        QueryCmd = "CREATE TABLE IF NOT EXISTS product_descriptions (productid VARCHAR(100) NOT NULL, description TEXT, isdefault CHAR(1), type CHAR(1), localeid CHAR(1));"
                    }
                },
                { "product_featurebullets", new SQLTable
                    {
                        Filename = "EN_US_B_productfeaturebullets.csv",
                        QueryCmd = "CREATE TABLE IF NOT EXISTS product_featurebullets (productid INT, ordernumber SMALLINT, localeid SMALLINT, text TEXT, modifieddate TIMESTAMP);"
                    }
                },
                { "product_locales", new SQLTable
                    {
                        Filename = "EN_US_B_productlocales.csv",
                        QueryCmd = "CREATE TABLE IF NOT EXISTS product_locales (productid INT, isactive CHAR(1), published TINYTEXT, PRIMARY KEY(productid));"
                    }
                },
                {  "product_accessories", new SQLTable
                    {
                        Filename = "EN_US_A_productaccessories.csv",
                        QueryCmd = "CREATE TABLE IF NOT EXISTS product_accessories (productid INT, accessoryid INT, localeid SMALLINT);"
                    }
                },
                {  "search_attributes", new SQLTable
                    {
                        Filename = "EN_US_B_searchattributes.csv",
                        QueryCmd = "CREATE TABLE IF NOT EXISTS search_attributes (productid INT, categoryid INT, unknownid INT, isactive SMALLINT, localeid SMALLINT);"
                    }
                },
                {  "product_keywords", new SQLTable
                    {
                        Filename = "EN_US_B_productkeywords.csv",
                        QueryCmd = "CREATE TABLE IF NOT EXISTS product_keywords (productid INT, text TEXT, localeid SMALLINT);"
                    }
                }
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
                ParseSpexDirectory(tables, conn);

                conn.Close();
                Logger("[MySQL] Connection closed\n");
            }

            DeleteSpexDirectory();
        }

        static void ParseSpexDirectory(dynamic tables, MySqlConnection conn)
        {
            string dir = Directory.GetCurrentDirectory();
            string[] files = Directory.GetFiles(dir + @"\spex\", "*.csv");

            for (int i = 0; i < files.Length; i++)
            {
                string file = Path.GetFileName(files[i]);
                foreach (var sqldata in tables)
                {
                    if (sqldata.Value.Filename == file)
                    {
                        Logger("[MySQL] Importing " + file + "...");
                        ImportSpexData(conn, sqldata.Key, sqldata.Value);
                        Logger(" Done\n");
                        break;
                    }
                }
            }
        }

        static void ImportSpexData(MySqlConnection conn, string tablename, dynamic values)
        {
            string queryCmd = String.Format("DROP TABLE IF EXISTS {0}", tablename);
            using (MySqlCommand cmd = new MySqlCommand(queryCmd, conn))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            //Create the TABLE
            using (MySqlCommand cmd = new MySqlCommand(values.QueryCmd, conn))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            string dir = Directory.GetCurrentDirectory();
  
            MySqlBulkLoader bulk = new MySqlBulkLoader(conn);
            bulk.TableName = tablename;
            bulk.FieldTerminator = ",";
            bulk.FieldQuotationOptional = false;
            bulk.FieldQuotationCharacter = '"';
            bulk.CharacterSet = "LATIN1";
            bulk.FileName = dir + @"\spex\" + values.Filename;
            bulk.Local = true;
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

        static void Logger(string message, bool bwrite=true)
        {
            string file = @".\..\log.txt";

            using (StreamWriter w = File.AppendText(file))
            {
                w.Write($"[{DateTime.Now.ToShortDateString()} @ {DateTime.Now.ToLongTimeString()}]");
                w.WriteLine(" " + message);
            }

            if (bwrite)
                Console.Write(message);
        }
    }
}
