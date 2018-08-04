using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    class Timing
    {
        DateTime _start;
        DateTime _end;
        TimeSpan duration;
        Db db;
        public Timing(Db db)
        {
            this.db = db;
            start();
        }

        public void start()
        {
            _start = DateTime.Now;
        }
        public void stop()
        {
            _end = DateTime.Now;
            duration = _end.Subtract(_start);
        }
        public string getStart()
        {
            return _start.ToString("yyyy-MM-dd HH:mm:ss");  // msaccess can't handle decimals no .fff
        }
        public string getEnd()
        {
            return _end.ToString("yyyy-MM-dd HH:mm:ss");    // msaccess can't handle decimals no .fff
        }
        public void log(string query)
        {
            if (db != null && db.logging)
            {
                db.logging = false; // stop loggining logs
                db.insert("timings", "start,end,duration,query", 
                    $"#{getStart()}#,#{getEnd()}#,{duration.TotalMilliseconds},{App.sqlStringValueMarker}'{query}'");
                db.logging = true;

            } else
            {
                query += "  - not logged";
            }
            // don't output the logging queries
            if(! query.StartsWith("INSERT INTO [timings]"))
                Console.WriteLine($"duration : { duration.TotalMilliseconds}, query : \"{query}\" ");

        }
            
    }
}
