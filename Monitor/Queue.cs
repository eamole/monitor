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

            OleDbDataReader reader = App.snapshot.query("queues", "", "priority");
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
            Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
            start();
        }
        public void start()
        {
            // load the next file
            string table = "tables"; 
            string where = $"queueId={id} and queueRun<{queueRun}";
            int count = App.snapshot.recCount(table,where) ;
            if(count==0)
            {
                // next run
                queueRun++;
                where = $"queueId={id} and queueRun<{queueRun}";
                save();
            }

            OleDbDataReader reader = App.snapshot.query(table,where,"priority");
            if(reader.HasRows)
            {
                // still jobs in queue
                reader.Read();
                // change back to tableId when using queueTables
                int tableId = int.Parse(reader.GetValue(reader.GetOrdinal("id")).ToString());
                //string tableName = App.snapshot.lookup("tables", tableId,null,"id");
                string tableName = reader.GetValue(reader.GetOrdinal("name")).ToString();

                Table.checkForInserts(tableName);
                // only on success
                App.snapshot.update(table, "queueRun", $"{queueRun}", $"queueId={id}");

                // **** now set the wait
                timer.Start();
            }
            else
            {
                // guessing an empty queue!!
            }

        }
        public void save()
        {
            string fields = "name,priority,queueRun";
            string values = $"\"{name}\" , {priority} , {queueRun}";
            App.snapshot.upsert("queues", fields, values,$"id = {id}");
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
