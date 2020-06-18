using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpexImport
{
    class SQLData
    {
        public string fileName;
        public string creationQuery;
        public string primaryKey;
        public string foreignKeys;

        public SQLData(string file, string query)
        {
            fileName = file;
            creationQuery = query;
        }
    }

    struct SQLQueries
    { 
        
    }

    class SpexSQL
    {
    }
}