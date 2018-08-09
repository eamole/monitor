using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using System.Timers;

namespace Monitor
{

    class Queue 
    {
        public static Dictionary<int, Queue> queues;
        public int id;
        public string name;
        public int priority;
        public int timeBetweenRuns;
        public int queueRun;
        public Thread thread;
        public System.Timers.Timer timer;

        public static void loadQueues()
        {
            queues = new Dictionary<int, Queue>();
            App.log("load  queues");
            OleDbDataReader reader = App.snapshotDb.query("queues", "", "priority");
            while(reader.Read())
            {
                Queue q = new Queue(reader);
                queues.Add(q.id, q);

                q.thread = new Thread(new ThreadStart(q.start));
                q.thread.Start();

            }
        }

        public Queue(OleDbDataReader reader)
        {
            read(reader);
            timer = new System.Timers.Timer(timeBetweenRuns);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = false;
            //timer.Start();    
        }
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            App.log("queue wakeup " + e.SignalTime);
            
            next();
        }
        public void start()
        {
            App.log("Queue Startup");

            App.log("wait for delay before starting queue processing");
            timer.Start();

        }

        public void next()
        {
            App.log("next file");
            // load the next file
            string tables = "tables";
            string where = $"queueId={id} and queueRun<{queueRun}";
            int count = App.snapshotDb.recCount(tables, where);
            if (count == 0)
            {
                App.log("queue processed/empty, next queue run");
                // next run
                queueRun++;
                where = $"queueId={id} and queueRun<{queueRun}";
                save();
            }
            // sort by high numbers first - by default tables will by 0
            OleDbDataReader reader = App.snapshotDb.query(tables, where, "priority DESC");
            if (reader.HasRows)
            {
                // still jobs in queue
                App.log("jobs in queue");

                reader.Read();  // load the table details

                string sql;
                
                // change back to tableId when using queueTables
                int tableId = int.Parse(reader.GetValue(reader.GetOrdinal("id")).ToString());
                //string tableName = App.snapshot.lookup("tables", tableId,null,"id");
                string tableName = reader.GetValue(reader.GetOrdinal("name")).ToString();
                reader.Close(); // 

                Table table = App.allDb.tables[tableName];
                // once per pass
                table.getRecCount();
                // need to retain the query objects
                App.log("look for updates " + tableName);
                Table.checkForUpdates(id,tableName);                
                
                App.log("look for inserts " + tableName);
                Table.checkForInserts(id,tableName);

                App.log("look for deletes " + tableName);
                Table.checkForInserts(id,tableName);

                // only on success
                App.log("move this table to next run");
                // TODO change to tableid qhen using queueTables
                // starnge I didn't get a query error on tableId I think tableId has meaning in msaccess!! It just hung!!
                App.snapshotDb.update(tables, "queueRun", $"{queueRun}", $"queueId={id} AND id={tableId}");

                App.log("queue pause");
                // **** now set the wait
                timer.Start();
            }
            else
            {
                App.log("looks like no tables in this queue at all");
                // guessing an empty queue!!
            }

        }
        public void save()
        {
            string fields = "name,priority,queueRun";
            string values = $"\"{name}\" , {priority} , {queueRun}";
            App.snapshotDb.upsert("queues", fields, values,$"id = {id}");
        }


        public void read(OleDbDataReader reader)
        {
            /*
             * jaysus - these all fail on NULLs
             * need a better way of reading records!! Maybe pass an array in of type
             * bind name of field to property of same name
             */
            id = int.Parse(reader.GetValue(reader.GetOrdinal("id")).ToString());
            name = reader.GetValue(reader.GetOrdinal("name")).ToString();
            priority = int.Parse(reader.GetValue(reader.GetOrdinal("priority")).ToString());
            timeBetweenRuns = int.Parse(reader.GetValue(reader.GetOrdinal("timeBetweenRuns")).ToString());
            queueRun = int.Parse(reader.GetValue(reader.GetOrdinal("queueRun")).ToString());


        }


    }
}
