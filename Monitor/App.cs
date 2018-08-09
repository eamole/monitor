using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;

namespace Monitor
{
    class App
    {
        public static int queryTimerInterval = 3000; // ms only for long running queries
        public static int queryMaxRunTime = 10*1000; // 30 seconds ms only for long running queries
        public static int queryMaxRetries = 2;      // max number of times a query is rerun, before being blocked
        public static bool useTableLastUpdateInQueries = false;
        public static int maxFieldListSize = 30;
        public static int maxIdArraySize = 500;
        public static string fieldListSeparator = "chr(124)";   // its a sql function
        public static char fieldListSeparatorChar = (char) 124; // should be pipe |

        // used in a value list to denote an sql string or any struing that should be parameterised in a query
        public static char sqlStringValueMarker = '~';
        public static char sqlStringValueDelim = '~';
        public static string dateMask = "yyyy-MM-dd";  
        public static string timeMask = "HH:mm:ss"; // 24 hour clock
        public static string dateTimeMask = dateMask + "T" + timeMask + "Z";    // ISO std good for JSON 
        
        // the 3 dbs in the set
        public static Db originalDb;
        public static Snapshot snapshotDb;
        public static Db loggerDb;
        public static Snapshot allDb; // all tables linked

        public static string appPath;
        public static string dataPath;      // set by Snapshot - really should be set afetr selecting the database
        public static string deltaPath;      // set by Snapshot



        public static void init()
        {
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = App.dateMask ;
            culture.DateTimeFormat.LongTimePattern = App.timeMask;
            Thread.CurrentThread.CurrentCulture = culture;
            Console.WriteLine(DateTime.Now);

        }

        public static void log(string msg)
        {
            Console.WriteLine(msg);
        }
        public static void error(string msg)
        {
            Console.WriteLine("Error : " + msg);    // send email
            App.loggerDb.insert("errors", "errMsg", $"'{msg}'");
        }





    }

}
