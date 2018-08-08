using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace Monitor
{
    public enum states { Blocked,Initialising, Running, Done };
    // set the value of result
    public enum results { Success, Stopped, Error };

    /*
     * we need to statardise the reporting of queries. Also need to be able to handle
     * long running queries, and queries that seem to have stoped or are blocking
     * 
     */
    class Query : Timing
    {
        public Db db;
        public string name; // must name queries? or is it an option
        public string sql;
        public System.Timers.Timer timer;
        public OleDbCommand cmd;
        public int rowsAffected;    // set by commands
        public string errorMsg;
        public states state;
        public results result;
        public int retries = 0;
        public int _runtime;
        public OleDbDataReader _reader;

                                        // log to snapshot
        public Query(Db db,string sql=""): base(App.loggerDb)
        {
            this.db = db;
            this.sql = sql;
            timer = new System.Timers.Timer(App.queryTimerInterval);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = false;
            state = states.Initialising;

        }
        public static Query run(Db db, string sql)
        {
            Query q = new Query(db, sql);
            q.execute();
            return q;

        }
        public static Query reader(Db db, string sql)
        {
            Query q = new Query(db, sql);
            q._reader = q.reader();
            return q;
        }
        public int runtime()
        {
            TimeSpan runtime = DateTime.Now.Subtract(_start);
            _runtime = (int) runtime.TotalMilliseconds;
            return _runtime;
        }
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            App.log($"Query status check @ {e.SignalTime} | Runtime : {runtime()} | Max RunTime : {App.queryMaxRunTime}");
            if(state == states.Running)
            {
                if(runtime() > App.queryMaxRunTime)
                {
                    App.log($"need to cancel query.| Runtime : {runtime()} | Max RunTime : {App.queryMaxRunTime} ");
                    Timing timing = new Timing(this.db);
                    cmd.Cancel();
                    timing.stop();
                    timing.log("Cancelling Query");
                    state = states.Done;
                    result = results.Stopped;
                    timer.Stop();
                    throw new TimeoutException($"SQL Query exceeded maximum allowed runtime. | Runtime : {runtime()} | Max RunTime : {App.queryMaxRunTime} ") ;
                    // I should save/log this efffort and allow a retry - now while in memory?
                    // or leave the scheduler retry
                } else
                {
                    timer.Start();  // schedule another run
                }
            } else
            {
                App.log($@"This query should have stopped. Will stop 
                            {errorMsg}
                            {sql}");
                timer.Stop();
            }
        }

        new private void start()
        {
            App.log("start query");
            base.start();   // timing;
            timer.Start();
            state = states.Running;
        }

        new private void stop()
        {
            App.log("query complete");
            base.stop();    // timing
            timer.Stop();   // cancel timer
            state = states.Done;
            result = results.Success;
            // need to save this object

        }
        public void error(Exception e)
        {
            App.error($@"*** SQL Query Reader : {e.Message}
                            {sql}");
            errorMsg = e.Message;

            // allow retry?
            state = states.Blocked;
            result = results.Error;
            // need to save this object

        }

        public OleDbDataReader reader()
        {
            if (!db.connected) db.connect();

            start();
            cmd = new OleDbCommand(sql, db.conn);
            OleDbDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
                stop();
                log(sql);

            }
            catch (Exception e)
            {
                error(e);
            }

            return reader;
        }
        public bool read()
        {
            bool more = false;
            try
            {
                more = _reader.Read();
            } catch(Exception e)
            {
                App.error("Something wrong with SQL Reader : " + e.Message);
            }
            return more;
        }



        /*
         * could possiby do these with Generics
         */
        public string get(string name)
        {
            string value = _reader.GetValue(_reader.GetOrdinal(name)).ToString();
            return value;
        }
        public string getString(string name)
        {
            string value = get(name);
            return value;
        }

        public int getInt(string name)
        {
            int value = int.Parse(get(name));
            return value;
        }

        public DateTime getDateTime(string name)
        {
            DateTime value = DateTime.Parse(get(name));
            return value;
        }
        public bool getBool(string name)
        {
            bool value = bool.Parse(get(name));
            return value;
        }


        public int execute()
        {
            if (!db.connected) db.connect();

            start();
            cmd = new OleDbCommand(sql, db.conn);
            App.log("exec query " + sql);
            rowsAffected = 0;
            try
            {
                rowsAffected = cmd.ExecuteNonQuery();
                App.log("done");
                stop();
                log(sql);
            }
            catch (Exception e)
            {
                error(e);
            }
            return rowsAffected;
        }



    }
}
