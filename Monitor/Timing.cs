using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    class Timing
    {
        public DateTime _start;
        public DateTime _end;
        public TimeSpan duration;
        public Db db;

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
        public double getDuration()
        {
            return duration.TotalMilliseconds;
        }
        public void log(string query)
        {
            if (db != null && db.logging)
            {
                db.logging = false; // stop loggining logs
                //db.insert("timings", "start,end,duration,query", 
                //    new object[]
                //    {
                //        _start ,_end,duration.TotalMilliseconds,query
                //    }
                //);
                query = query.Replace("\r\n", " "); // get rid of new lines
                db.insert("timings", "start,end,duration,query",
                        $@"#{getStart()}#,
                            #{getEnd()}#,
                            {getDuration()},
                            {App.sqlStringValueDelim}{query}{App.sqlStringValueDelim}"
                );

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
