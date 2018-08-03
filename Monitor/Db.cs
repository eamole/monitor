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
            setLog(log);
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
            // msaccess doesn't support
            //connStr += "MultipleActiveResultSets = true;";
            connectionString = connStr;

            tables = new Dictionary<string, Table>();

        }

        public void setLog(Db log=null)
        {
            if (log != null) this.log = log;
            timing = new Timing(log);   // log may be null

        }

        // destructor
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
        
        public OleDbDataReader sql(string sql)
        {
            if (!connected) connect();

            timing.start();
            cmd = new OleDbCommand(sql, conn);

            OleDbDataReader reader = cmd.ExecuteReader();

            timing.stop();
            timing.log(sql);

            return reader;
        }

        public OleDbDataReader join(string fields , string tables, string on, string where = "", string orderBy = "")
        {
            string[] _tables = tables.Split(',');
            string sql = $"SELECT {fields} FROM {_tables[0]} ";
            sql += $" INNER JOIN {_tables[1]} ";
            sql += $" ON ( {on} )";

            if (where.Length > 0)
            {
                sql += $" WHERE {where} ";
            }
            if (orderBy.Length > 0)
            {
                sql += $" ORDER BY {orderBy} ";
            }
            sql += ";";
            if (!connected) connect();

            timing.start();
            cmd = new OleDbCommand(sql, conn);

            OleDbDataReader reader = cmd.ExecuteReader();

            timing.stop();
            timing.log(sql);

            return reader;

        }

        public OleDbDataReader query(string table,string where = "",string orderBy="")
        {
            string sql = $"SELECT * FROM {table} ";
            if(where.Length>0)
            {
                sql += $" WHERE {where} ";
            }
            if (orderBy.Length > 0)
            {
                sql += $" ORDER BY {orderBy} ";
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
        /*
         * use a table name to get just one schema
         */
        public DataTable getTables(string tableName = null)
        {
            if (!connected) connect();

            timing.start();
            DataTable result = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, tableName, "TABLE" });
            timing.stop();
            timing.log("GetDatabaseSchema - TABLES ");
            return result;
        }

        public DataTable getFields(string table)
        {
            if (!connected) connect();

            timing.start();
            DataTable result = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, table});
            timing.stop();
            if(log.conn.State==ConnectionState.Open)
                timing.log($"GetTableSchema - {table} ");
            return result;
        }

        public void update(string tableName, string fields, string values , string where )
        {
            string[] _fields = fields.Split(',');
            // this will fuck with function calls and expressions
            char sep = ',';
            if (values[0]=='|')
            {
                sep = '|';
                values = values.Substring(1);
            } 
            string[] _values = values.Split(sep);
            
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
        public void delete(string tableName, string where = "", bool confirmAll = false)
        {
            string _sql = $"DELETE * FROM {tableName} ";
            if(where.Length==0)
            {
                if (!confirmAll)
                {
                    Console.WriteLine($"*** SQL Delete table {tableName} : You must confirm All if no where clause specified");
                    return;
                } else
                {
                    Console.WriteLine($"*** SQL Delete : Warning - deleting all records table : {tableName}");
                }
            } else
            {
                _sql += $" WHERE {where} ;";
            }
            sql(_sql);

        }
        // update if exists, insert if not - where must be key
        public void upsert(string tableName, string fields, string values, string where)
        {
            int count = recCount(tableName, where);
            if(count==0)
            {
                insert(tableName, fields, values);
            } else if(count == 1)
            {
                update(tableName, fields, values, where);
            } else
            {
                Console.WriteLine($@"*** Upsert Non Unique Warning. 
                        {count} Records satsify where clause : {tableName} : WHERE {where} ");
            }

        }
        public string lookup(string table,int id, string field = "name", string idField = "_id" )
        {
            string value = "";
            OleDbDataReader reader = query(table,$"{idField} = {id}");
            if(reader.HasRows)
            {
                reader.Read();
                value = reader.GetValue(reader.GetOrdinal(field)).ToString();
            }
            return value;
        }
        public void inc(string table, int id, string field, string idField = "_id")
        {
            update(table, field, $"{field}+1", $"idField={id}");
        }
        public void insert(string tableName , string fields , string values)
        {
            string[] _fields = fields.Split(',');
            fields = String.Join(",",_fields.Select(x => $"[{x}]").ToArray());

            // this will fuck with function calls and expressions
            char sep = ',';
            if (values[0] == '|')
            {
                sep = '|';
                values = values.Substring(1);
            }
            string[] _values = values.Split(sep);
            // need to rejoin them!! this is simply for compat with update
            values = String.Join(",", _values);

            string sql = $"INSERT INTO [{tableName}] ({fields}) VALUES ({values})";
            if (!connected) connect();

            timing.start();
            cmd = new OleDbCommand(sql, conn);
            cmd.ExecuteNonQuery();
            timing.stop();
            timing.log(sql);
        }

        public static Table fromAdoDb(Db db, DataRow tableMeta)
        {
            string tableName = tableMeta["TABLE_NAME"].ToString();
            Table table = new Table(db, tableName);
            table.created = DateTime.Parse(tableMeta["DATE_CREATED"].ToString());
            table.updated = DateTime.Parse(tableMeta["DATE_MODIFIED"].ToString());

            table.getRecCount();
            table.adodbGetFields();

            return table;
        }

        public Dictionary<string,string> tablesToJson()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            DataTable table = getTables();

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

        public Int32 recCount(string table,string where="")
        {
            string sql = $"SELECT COUNT(*) FROM [{table}] ";
            if (where.Length > 0) sql += $" WHERE {where} ";
            sql += ";";
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
