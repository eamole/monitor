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
        public static int maxFieldListSize = 30;
        public static int maxIdArraySize = 500;
        public static string fieldListSeparator = "chr(124)";   // its a sql function
        // used in a value list to denote an sql string or any struing that should be parameterised in a query
        public static char sqlStringValueMarker = '~';
        public static char sqlStringValueDelim = '~';
        public static string dateMask = "yyyy-MM-dd";  
        public static string timeMask = "HH:mm:ss"; // 24 hour clock
        // the 3 dbs in the set
        public static Db originalDb;
        public static Snapshot snapshot;
        public static Db logger;

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
        }


    }

}
