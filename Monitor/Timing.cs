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
        public void log(string msg)
        {
            if (db != null)
            {
                db.insert("timings", "start,end,duration,query", $"#{getStart()}#,#{getEnd()}#,{duration.TotalMilliseconds},'{msg}'");
            }
            Console.WriteLine($"start : { getStart()} , end : { getEnd() }, duration : { duration.TotalMilliseconds}, query : \"{msg}\" ");

        }
            
    }
}
