using System;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.VisualBasic.FileIO;

namespace SpexImport
{
    class Import
    {
        private static string db_host = "localhost";
        private static string db_user = "spex";
        private static string db_pass = "spex";
        private static string db_name = "Spex";
        private static string db_port = "5432";

        static void Main(string[] args)
        {
            //DownloadFromFTP("ftp://ftp.etilize.com/IT_CE/content/EN_US/basic/basic_EN_US_current_mysql.zip");
            ConnectToDatabase();
            System.Environment.Exit(0);
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
            string connString = String.Format("Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
                db_host,
                db_user,
                db_name,
                db_port,
                db_pass);

            using (var conn = new NpgsqlConnection(connString))
            {
                Console.WriteLine("Establishing connection to PostgreSQL Database");
                conn.Open();

                using (var command = new NpgsqlCommand("DROP TABLE IF EXISTS products", conn))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new NpgsqlCommand("CREATE TABLE products (product_id VARCHAR(32) NOT NULL, manufacture_id VARCHAR(32), is_active boolean, mfg_pn VARCHAR(32) NOT NULL, category_id INT, is_accessory boolean, equivalency NUMERIC, create_date TIMESTAMP, modify_date TIMESTAMP, last_update TIMESTAMP, PRIMARY KEY(product_id, mfg_pn));", conn))
                {
                    command.ExecuteNonQuery();
                    ImportProductsToDatabase();
                }
            }
        }

        static void ImportProductsToDatabase()
        {
            string file = @".\spex\EN_US_B_product.csv";

            using (TextFieldParser parser = new TextFieldParser(file))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                }
            }
        }
    }
}
