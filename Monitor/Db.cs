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
        public Db log;
        public Timing timing;
        public Dictionary<string, Table> tables;

        public Db(string _filename = null, Db log = null)
        {
            if (log != null) this.log = log;
            timing = new Timing(log);   // log may be null

            provider = Properties.Settings.Default.DatabaseProvider;
            if (_filename == null)
            {
                filename = Properties.Settings.Default.DatabaseFilename;
            } else
            {
                filename = _filename;
            }
            password = Properties.Settings.Default.DatabasePassword;

            string connStr = $"Provider={provider};";
            connStr += $"Data Source={filename};";
            if(password.Length>0)
            {
                connStr += $"Jet OLEDB:Database Password={password};";
            }
            connectionString = connStr;

            tables = new Dictionary<string, Table>();

        }
        ~Db() => close();
        public void close()
        {
            try
            {
                conn.Close();
            } catch(Exception e)
            {
                Console.WriteLine("Error closing DB Connection. Maybe already closed : " + e.Message );
            }
            connected = false;
        }
        public void connect()
        {
            timing.start();
            conn = new OleDbConnection(connectionString);
            conn.Open();
            connected = true;
            timing.stop();
            timing.log("connect : " + filename);
        }

        public OleDbDataReader query(string table,string where = "")
        {
            string sql = $"SELECT * FROM {table} ";
            if(where.Length>0)
            {
                sql += $" WHERE {where} ";
            }
            sql += ";";
            if(!connected) connect();
            
            timing.start();
            cmd = new OleDbCommand(sql , conn);
            
            OleDbDataReader reader = cmd.ExecuteReader();

            timing.stop();
            timing.log(sql);

            return reader;
        }

        public DataTable adodbGetTables()
        {
            if (!connected) connect();

            timing.start();
            DataTable result = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            timing.stop();
            timing.log("GetDatabaseSchema - TABLES ");
            return result;
        }

        public DataTable adodbGetFields(string table)
        {
            if (!connected) connect();

            timing.start();
            DataTable result = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, table});
            timing.stop();
            timing.log($"GetTableSchema - {table} ");
            return result;
        }

        public void update(string tableName, string fields, string values , string where )
        {
            string[] _fields = fields.Split(',');
            string[] _values = values.Split(',');
            update(tableName, _fields, _values, where);
        }
        public void update(string tableName, string[] fields, string[] values, string where)
        {
            string sql = $"UPDATE [{tableName}] SET ";
            int i = 0;
            foreach (string field in fields)
            {
                if (i > 0)
                {
                    sql += ",";
                }
                string value = values[i];
                i++;
                sql += $"[{field}] = {value}";
                
            }
            sql += $" WHERE {where} ;";

            if (!connected) connect();

            timing.start();
            cmd = new OleDbCommand(sql, conn);
            cmd.ExecuteNonQuery();
            timing.stop();
            timing.log(sql);

        }

        public void insert(string tableName , string fields , string values)
        {
            string[] _fields = fields.Split(',');
            fields = String.Join(",",_fields.Select(x => $"[{x}]").ToArray());

            string sql = $"INSERT INTO [{tableName}] ({fields}) VALUES ({values})";
            if (!connected) connect();

            timing.start();
            cmd = new OleDbCommand(sql, conn);
            cmd.ExecuteNonQuery();
            timing.stop();
            timing.log(sql);
        }

        public Dictionary<string,string> tablesToJson()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            DataTable table = adodbGetTables();

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
            string sql = $"SELECT COUNT(*) FROM [{table}];";
            if (!connected) connect();
            Int32 result;
            timing.start();
            cmd = new OleDbCommand(sql, conn);
            try
            {
                result = (Int32)cmd.ExecuteScalar();
            } catch(Exception e)
            {
                sql = "*** Exception SQL : " + e.Message + "\n" + sql;
                result = 0;
            }
            
            timing.stop();
            timing.log(sql);

            return result;

        }



    }
}
