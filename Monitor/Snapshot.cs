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
        Db originalDb;
        
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
            /*
             * the snapshot class is bound to the snapshot database - no log
             * the snapshot also contains the original database, and acts as the logging database for it
             */
            
            originalDb = new Db(originalFileName, this);

            
        }

        public void fromAdoDb()
        {
            DataTable _tables = originalDb.adodbGetTables();

            foreach (DataRow row in _tables.Rows)
            {

                string tableName = row["TABLE_NAME"].ToString();
                
                Table table = Table.fromAdoDb(this,row);
                tables.Add(tableName, table);
                table.save();
            }



        }
    }
}
