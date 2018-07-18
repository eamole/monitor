using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace Monitor
{
    class Db
    {
        public OleDbCommand cmd;
        public OleDbConnection conn;
        public string provider;
        public string filename;
        public string password;
        public string connectionString;
        public bool connected;

        public Db()
        {
            provider = Properties.Settings.Default.DatabaseProvider;
            filename = Properties.Settings.Default.DatabaseFilename;
            password = Properties.Settings.Default.DatabasePassword;

            string connStr = $"Provider={provider};";
            connStr += $"Data Source={filename};";
            if(password.Length>0)
            {
                connStr += $"Jet OLEDB:Database Password={password};";
            }
            connectionString = connStr;
            
        }
        ~Db() => close();
        public void close()
        {
            conn.Close();
            connected = false;
        }
        public void connect()
        {
            conn = new OleDbConnection(connectionString);
            conn.Open();
            connected = true;
        }

        public OleDbDataReader query(string table,string where = "")
        {
            string sql = @"SELECT * FROM {table} ";
            if(where.Length>0)
            {
                sql += " WHERE {where} ";
            }
            sql += ";";
            if(!connected)
            {
                connect();
            }
            cmd = new OleDbCommand(sql , conn);

            return cmd.ExecuteReader();
        }

        public DataTable tables()
        {
            if (!connected) connect();

            
            return conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

        }

        public Dictionary<string,string> tablesToJson()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            DataTable table = tables();

            foreach(DataRow row in table.Rows)
            {
                Console.WriteLine("");
                string tableName = row["TABLE_NAME"].ToString();
                foreach(DataColumn col in table.Columns)
                {
                    object cell = row[col];
                    Console.Write($" {col.ColumnName} : \"{cell}\"");
                }
                Console.Write(" Records : " + recCount(tableName));
            }

            return dict;
        }

        public Int32 recCount(string table)
        {
            string sql = $"SELECT COUNT(*) FROM {table};";
            if (!connected) connect();
            cmd = new OleDbCommand(sql, conn);

            return (Int32) cmd.ExecuteScalar();
        }



    }
}
