using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace Monitor
{
    class Snapshot : Db
    {
        public Db originalDb;
        
        public static Snapshot getInstance(string filename)
        {
            // need to extract the mdb first from filename

            string _filename = Path.GetFileNameWithoutExtension(filename);
            string snapshotFileName = Path.GetDirectoryName(filename) + "\\" +  _filename + "_snapshot.mdb";
            string originalFileName = filename;

            return new Snapshot(originalFileName, snapshotFileName);
        }
        public Snapshot(string originalFileName,string snapshotFileName) : base(snapshotFileName)
        {
            //setLog(this);
            /*
             * the snapshot class is bound to the snapshot database - no log
             * the snapshot also contains the original database, and acts as the logging database for it
             */
            
            originalDb = new Db(originalFileName, this);

            
        }

        public void fromAdoDb()
        {
            DataTable _tables = originalDb.getTables();

            foreach (DataRow row in _tables.Rows)
            {

                string tableName = row["TABLE_NAME"].ToString();
                
                Table table = Table.fromAdoDb(row);
                tables.Add(tableName, table);
                table.save();
            }

        }
        /*
         * load the table data from the snapshot database
         * also return it as a datasource for a grid
         */
        public DataTable fromSnapshot()
        {
            OleDbDataReader  reader = query("tables");
            OleDbCommand tmp = cmd;
            while(reader.Read())
            {
                Table.getTable(reader);     // fromSnapshot()
            }
            reader.Close(); // need to close this reader first - why? is this two queries?

            OleDbDataAdapter da = new OleDbDataAdapter(tmp);
            DataTable _tables = new DataTable();

            da.Fill(_tables);
            return _tables;
        }

        public void insertIdFields()
        {
            foreach (KeyValuePair<string, Table> entry in tables)
            {
                Table table = entry.Value;
                table.insertIdField(this);

            }
        }
        public void allocateFieldsToLists()
        {
            // first allocate fields to lists within table;
            // need to join the field list

        }
        public void genFieldLists()
        {
            foreach (KeyValuePair<string, Table> entry in tables)
            {
                Table table = entry.Value;
                Table.genFieldLists(table.name);
            }
        }

        public void checkForInserts()
        {
            foreach (KeyValuePair<string, Table> entry in tables)
            {
                Table table = entry.Value;
                Table.checkForInserts(table.name);
            }

        }

        /*
         * add a new batch record, return the ID
         * this should only be called on snapshot
         * have to put it here so callable from table
         */
        public int getNewBatchId(string notes)
        {
            insert("batches", "notes", $"'{notes}'");
            OleDbDataReader reader = sql("SELECT MAX([id]) AS [MaxId] FROM [batches];");
            reader.Read();
            int id = int.Parse(reader.GetValue(reader.GetOrdinal("MaxId")).ToString());
            reader.Close();
            return id;
        }
        public int getLastBatchId()
        {
            OleDbDataReader reader = App.snapshot.sql("SELECT Max([id]) as [MaxId] FROM [batches];");
            reader.Read();
            int batchId = int.Parse(reader.GetValue(reader.GetOrdinal("MaxId")).ToString());
            reader.Close();
            return batchId;
        }
        public int getNewStats(string op,int tableId)
        {
            insert("tableStats", "op,tableId", $"'{op}',{tableId}");
            OleDbDataReader reader = sql("SELECT MAX([id]) AS [MaxId] FROM [tableStats];");
            reader.Read();
            int id = int.Parse(reader.GetValue(reader.GetOrdinal("MaxId")).ToString());
            reader.Close();
            return id;
        }
        public void recordStats(int id,int recCount)
        {
            update("tableStats", "recCount", $"{recCount}",$"[id]={id}");
        }

        public void computeFieldStats()
        {
            int batchId = getNewBatchId("Field Stats");

            /*
             * for each table
             * compute the number of null and non null fields
             * tables is the local load from the tables table
             */
            foreach (KeyValuePair<string,Table> entry in tables)
            {
                Table table = entry.Value;
                if(table.recCount>0)
                {
                    string query = table.computeFieldStatsQuery();
                    OleDbDataReader reader = originalDb.sql(query);

                    table.writeFieldStats(batchId,reader);
                } else
                {
                    table.writeFieldStats(batchId);
                }
                
            }
        }
    }
}
