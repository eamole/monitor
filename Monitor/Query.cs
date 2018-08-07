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

        public Query(Db db,string sql=""): base(App.snapshot)
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
            _runtime = DateTime.Now.Subtract(_start).Milliseconds;
            return _runtime;
        }
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            App.log("Query status check " + e.SignalTime);
            if(state == states.Running)
            {
                if(runtime() > App.queryMaxRunTime)
                {
                    cmd.Cancel();
                    state = states.Done;
                    result = results.Stopped;

                    // I should save/log this efffort and allow a retry - now while in memory?
                    // or leave the scheduler retry
                }
            }
        }

        new private void start()
        {
            base.start();   // timing;
            timer.Start();
            state = states.Running;
        }

        new private void stop()
        {
            base.stop();    // timing
            timer.Stop();   // cancel timer
            state = states.Done;
            result = results.Success;
            // need to save this object

        }
        public void error(Exception e)
        {
            App.error("SQL Query Reader : " + e.Message);
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
            } catch(Exception e)
            {
                error(e);
            }

            stop();
            log(sql);

            return reader;
        }
        public bool read()
        {
            return _reader.Read();
        }



        /*
         * could possiby do these with Generics
         */
        public string get(string name)
        {
            string value = _reader.GetValue(_reader.GetOrdinal(name)).ToString();
            return value;
        }

        public int get(string name)
        {
            int value = int.Parse(_reader.GetValue(_reader.GetOrdinal(name)).ToString());
            return value;
        }

        public DateTime get(string name)
        {
            DateTime value = DateTime.Parse(_reader.GetValue(_reader.GetOrdinal(name)).ToString());
            return value;
        }
        public bool get(string name)
        {
            bool value = bool.Parse(_reader.GetValue(_reader.GetOrdinal(name)).ToString());
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
            } catch(Exception e)
            {
                error(e);
            }
            App.log("done");
            stop();
            log(sql);
            return rowsAffected;
        }



    }
}
