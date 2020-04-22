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

namespace SpexImport
{
    class Import
    {
        private static string db_host, db_user, db_pass, db_name;
        private static uint db_port;

        static void Main(string[] args)
        {
            LoadConfiguration();
            //DownloadFromFTP("ftp://ftp.etilize.com/IT_CE/content/EN_US/basic/basic_EN_US_current_mysql.zip");
            ConnectToDatabase();
            //System.Environment.Exit(0);
            //Thread.Sleep(60 * 600);
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

        static void DownloadFromFTP(string url)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
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

            Console.WriteLine("Unzipped basic.zip successfully");

            ConnectToDatabase();
        }

        static void ConnectToDatabase()
        {
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
                Console.WriteLine("[SQL] Establsihing connection to MySQL");
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SQL] Connection was NOT established to MySQL");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine("[SQL] Importing product.csv...");
                ImportProduct(conn);

                Console.WriteLine("[SQL] Importing product_descriptions.csv...");
                ImportProductDescriptions(conn);

                Console.WriteLine("[SQL] Importing product_featurebullets.csv...");
                ImportProductFeatures(conn);

                conn.Close();
                Console.WriteLine("[SQL] Connection closed");
            }
        }

        static void ImportProduct(MySqlConnection conn)
        {
            var dir = Directory.GetCurrentDirectory();
            string file = @"\spex\EN_US_B_product.csv";

            using (MySqlCommand cmd = new MySqlCommand("DROP TABLE IF EXISTS product", conn))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            //Create the TABLE
            using (MySqlCommand cmd = new MySqlCommand("CREATE TABLE IF NOT EXISTS product (productid INT NOT NULL, mfgid VARCHAR(16), mfgpn VARCHAR(128) NOT NULL, categoryid INT, is_active CHAR(1), equivalency TEXT, create_date TIMESTAMP, modify_date TIMESTAMP, last_update TIMESTAMP, PRIMARY KEY(productid, mfgpn));", conn))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            //Bulk load CSV file
            MySqlBulkLoader bulk = new MySqlBulkLoader(conn);
            bulk.TableName = "product";
            bulk.FieldTerminator = ",";
            bulk.FieldQuotationOptional = false;
            bulk.FieldQuotationCharacter = '"';
            bulk.CharacterSet = "LATIN1";
            bulk.FileName = dir + file;
            bulk.Local = true;
            bulk.Load();
        }

        static void ImportProductDescriptions(MySqlConnection conn)
        {
            var dir = Directory.GetCurrentDirectory();
            string file = @"\spex\EN_US_B_productdescriptions.csv";

            using (MySqlCommand cmd = new MySqlCommand("DROP TABLE IF EXISTS product_descriptions", conn))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            //Create the TABLE
            using (MySqlCommand cmd = new MySqlCommand("CREATE TABLE IF NOT EXISTS product_descriptions (productid VARCHAR(100) NOT NULL, description TEXT, isdefault CHAR(1), type CHAR(1), localeid CHAR(1));", conn))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            /*string strCmd = String.Format("LOAD DATA LOCAL INFILE '{0}{1}' INTO TABLE spex.product_descriptions CHARACTER SET 'LATIN1' FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '{2}'", dir, file, "\"");
            using (MySqlCommand cmd = new MySqlCommand(strCmd, conn))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }*/
            MySqlBulkLoader bulk = new MySqlBulkLoader(conn);
            bulk.TableName = "product_descriptions";
            bulk.FieldTerminator = ",";
            bulk.FieldQuotationOptional = false;
            bulk.FieldQuotationCharacter = '"';
            bulk.CharacterSet = "LATIN1";
            bulk.FileName = dir + file;
            bulk.Local = true;
            bulk.Load();
        }

        static void ImportProductFeatures(MySqlConnection conn)
        {
            var dir = Directory.GetCurrentDirectory();
            string file = @"\spex\EN_US_B_productdescriptions.csv";

            using (MySqlCommand cmd = new MySqlCommand("DROP TABLE IF EXISTS product_featurebullets", conn))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            //Create the TABLE
            using (MySqlCommand cmd = new MySqlCommand("CREATE TABLE IF NOT EXISTS product_featurebullets (productid INT, ordernumber SMALLINT, localeid SMALLINT, text TEXT, modifieddate TIMESTAMP);", conn))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            MySqlBulkLoader bulk = new MySqlBulkLoader(conn);
            bulk.TableName = "product_featurebullets";
            bulk.FieldTerminator = ",";
            bulk.FieldQuotationOptional = false;
            bulk.FieldQuotationCharacter = '"';
            bulk.CharacterSet = "LATIN1";
            bulk.FileName = dir + file;
            bulk.Local = true;
            bulk.Load();
        }
    }
}
