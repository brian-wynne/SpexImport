using System;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using Microsoft.VisualBasic.FileIO;

using System.Configuration;
using System.Collections.Specialized;

// TODO: Download + unzip all needed files, put in to 'SPEX' directory. Cycle through directory and make a call for each file under an iter.
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

        static void Main(string[] args)
        {
            LoadConfiguration();

            Console.Write("[FTP] Downloading basic...");
            DownloadFromFTP("ftp://ftp.etilize.com/IT_CE/content/EN_US/basic/basic_EN_US_current_mysql.zip", "basic.zip");
            Console.Write(" Done\n");

            Console.Write("[FTP] Downloading accessories...");
            DownloadFromFTP("ftp://ftp.etilize.com/IT_CE/content/EN_US/accessories/accessories_EN_US_current_mysql.zip", "accessories.zip");
            Console.Write(" Done\n");

            Console.Write("[FTP] Extracting basic.zip...");
            UnzipCatalogContents("basic.zip");
            Console.Write(" Done\n");

            Console.Write("[FTP] Extracting accessories.zip...");
            UnzipCatalogContents("accessories.zip");
            Console.Write(" Done\n");

            ConnectToDatabase();
            //System.Environment.Exit(0);
        }

        static void LoadConfiguration()
        {
            db_host = ConfigurationManager.AppSettings["db_host"];
            db_user = ConfigurationManager.AppSettings["db_user"];
            db_pass = ConfigurationManager.AppSettings["db_pass"];
            db_name = ConfigurationManager.AppSettings["db_name"];

            string port = ConfigurationManager.AppSettings["db_port"];
            db_port = Convert.ToUInt32(port);
        }

        static void DownloadFromFTP(string url, string filename)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            request.Credentials = new NetworkCredential("dhecsdelta", "8q@Hsb3lta");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            FileStream file = File.Create(filename);
            byte[] buffer = new byte[32 * 1024];
            int read;
            
            while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                file.Write(buffer, 0, read);
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
                Console.WriteLine("[MySQL] Establishing connection to MySQL");
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[MySQL] Connection was NOT established to MySQL");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                ParseSpexDirectory(tables, conn);

                conn.Close();
                Console.WriteLine("[MySQL] Connection closed");
            }

            DeleteSpexDirectory();
        }

        static void ParseSpexDirectory(dynamic tables, MySqlConnection conn)
        {
            var dir = Directory.GetCurrentDirectory();
            string[] files = Directory.GetFiles(dir + @"\spex\", "*.csv");

            for (int i = 0; i < files.Length; i++)
            {
                string file = Path.GetFileName(files[i]);
                foreach (var sqldata in tables)
                {
                    if (sqldata.Value.Filename == file)
                    {
                        Console.Write("[MySQL] Importing " + file + "...");
                        ImportSpexData(conn, sqldata.Key, sqldata.Value);
                        Console.Write(" Done\n");
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

            var dir = Directory.GetCurrentDirectory();
            //Bulk load CSV file
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
            var dir = Directory.GetCurrentDirectory();
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
    }
}
