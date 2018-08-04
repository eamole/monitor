using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    class App
    {
        public static int maxFieldListSize = 30;
        public static int maxIdArraySize = 500;
        public static string fieldListSeparator = "chr(124)";   // its a sql function
        // used in a value list to denote an sql string or any struing that should be parameterised in a query
        public static char sqlStringValueMarker = '~';    
        public static Db originalDb;
        public static Snapshot snapshot;
    }
}
