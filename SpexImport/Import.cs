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
            UnzipCatalogContents();
            System.Environment.Exit(0);
        }

        static void DownloadFromFTP()
        {

        }

        static void UnzipCatalogContents()
        {
            string zipPath = @".\basic.zip";
            string extractPath = @".\spex";

            ZipFile.ExtractToDirectory(zipPath, extractPath);
        }
    }
}
