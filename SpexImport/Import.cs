﻿using System;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            //System.Environment.Exit(0);
            Thread.Sleep(60 * 600);
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
            string connString = String.Format("Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer;Pooling=False;Timeout=60;Command Timeout=1200;",
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

                using (var command = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS products (product_id VARCHAR(50) NOT NULL, mfg_id VARCHAR(16), mfg_pn VARCHAR(128) NOT NULL, category_id INT, is_active CHAR(1), equivalency TEXT, create_date TIMESTAMP, modify_date TIMESTAMP, last_update TIMESTAMP, PRIMARY KEY(product_id, mfg_pn));", conn))
                {
                    command.ExecuteNonQuery();
                }

                // Master import
                Console.WriteLine("[SQL] Importing products.csv");
                ImportProductsToDatabase(conn);

                using (var command = new NpgsqlCommand("DROP TABLE IF EXISTS products_descriptions", conn))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS products_descriptions (product_id VARCHAR(50) NOT NULL, description TEXT, is_default CHAR(1), type CHAR(1), locale_id CHAR(1), is_active CHAR(1), PRIMARY KEY(product_id));", conn))
                {
                    command.ExecuteNonQuery();
                }

                Console.WriteLine("[SQL] Importing product_descriptions.csv");
                //ImportProductDescriptionToDatabase(conn);
                conn.Close();
            }
        }

        static void ImportProductsToDatabase(NpgsqlConnection conn)
        {
            var dir = Directory.GetCurrentDirectory();
            string file = @"\spex\EN_US_B_product.csv";

            using (var command = new NpgsqlCommand("Set client_encoding = 'WIN1252';", conn))
            {
                command.ExecuteNonQuery();
            }

            string query_cmd = String.Format("COPY products FROM '{1}{0}' DELIMITER ',' CSV HEADER;", file, dir);

            Console.WriteLine(query_cmd);

            using (var command = new NpgsqlCommand(query_cmd, conn))
            {
                command.ExecuteNonQuery();
            }

            /*using (TextFieldParser reader = new TextFieldParser(file))
            {
                reader.HasFieldsEnclosedInQuotes = true;
                reader.SetDelimiters(",");

                while (!reader.EndOfData)
                {
                    string[] field = reader.ReadFields();

                    string teststr = String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", field[0], field[1], field[2], field[3], field[4], field[5], field[6], field[7], field[8]);
                    //Console.WriteLine(teststr);

                    string query_cmd = "COPY products FROM '{0}' DELIMITER ',' CSV HEADER;";
                    String.Format(query_cmd, file);

                    Console.WriteLine("Copied CSV");

                string query_cmd = "INSERT INTO products (product_id, mfg_id, mfg_pn, category_id, is_active, equivalency, create_date, modify_date, last_update) VALUES (@product_id, @mfg_id, @mfg_pn, @category_id, @is_active, @equivalency, @create_date, @modify_date, @last_update);";
                using (var command = new NpgsqlCommand(query_cmd, conn))
                {
                    Console.WriteLine(field[0] + field[3]);
                    int category_id = Int32.Parse(field[3]);
                    DateTime create_date = DateTime.Parse(field[6].ToString());
                    DateTime modify_date = DateTime.Parse(field[6].ToString());
                    DateTime last_update = DateTime.Parse(field[6].ToString());

                    command.Parameters.AddWithValue("product_id", field[0]);
                    command.Parameters.AddWithValue("mfg_id", field[1]);
                    command.Parameters.AddWithValue("mfg_pn", field[2]);
                    command.Parameters.AddWithValue("category_id", category_id);
                    command.Parameters.AddWithValue("is_active", field[4]);
                    command.Parameters.AddWithValue("equivalency", field[5]);
                    command.Parameters.AddWithValue("create_date", create_date);
                    command.Parameters.AddWithValue("modify_date", modify_date);
                    command.Parameters.AddWithValue("last_update", last_update);

                    command.ExecuteNonQuery();
                }
            }*/
        }

        static void ImportProductDescriptionToDatabase(NpgsqlConnection conn)
        {
            var dir = Directory.GetCurrentDirectory();
            string file = @"\spex\EN_US_B_product.csv";
        }
    }
}
