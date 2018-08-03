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
        public static Db originalDb;
        public static Snapshot snapshot;
    }
}
