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
        public bool logging = true;
        public Timing timing;

        public int rowsAffected;    // set by commands

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
            if (log != null)
            {
                this.log = log;
                logging = true;
            }
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
            App.log("exec query " + sql);
            cmd.ExecuteNonQuery();
            App.log("done");
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
        /*
         * string of fileds with an array of values
         */
        public void insert(string tableName, string fields, object[] values)
        {
            insert(tableName, fields.Split(','), values);
        }
        public void insert(string tableName, string[] fields, object[] values)
        {

            string _fields = String.Join(",", fields);
            string _values = "";
            for (int i=0;i<values.Length;i++)
            {
                if (i > 0) _values += ",";
                _values += $"@"+fields[i];      // param placeholders
            }

            string sql = $"INSERT INTO [{tableName}] ({_fields}) VALUES ({_values})";

            if (!connected) connect();
            cmd = new OleDbCommand(sql, conn);
            // add the params
            int _i = 0;
            foreach(object value in values) { 
                cmd.Parameters.AddWithValue($"@"+fields[_i], values[_i]);
                _i++;
            }

            rowsAffected = cmd.ExecuteNonQuery();
            timing.stop();
            timing.log(sql);

        }
        public void insert(string tableName , string fields , string values)
        {
            string[] _fields = fields.Split(',');
            fields = String.Join(",",_fields.Select(x => $"[{x}]").ToArray());

            // another kludge for sql strings - 
            // I'll assume 1 delimited by a special pair of chars - like sql dates
            // ~sql~
            // split string BEFORE break into commas
            var hasSql = false;
            var sqlQuery = "";
            if (values.Contains(App.sqlStringValueDelim))
            {
                hasSql = true;
                string[] temp = values.Split(App.sqlStringValueDelim);
                    // assuming 1 sql strung only, so 3 parts
                values = temp[0] + "@sql" + temp[2];
                sqlQuery = temp[1];
            }

            // this will fuck with function calls and expressions
            char sep = ',';
            if (values[0] == '|')
            {
                sep = '|';
                values = values.Substring(1);
            }

            // now we can split the values on , without worrying about 's in sql strings
            // damn - I coulda used the | hack I wrote earlier!!
            string[] _values = values.Split(sep);




            // temp array for params
            string[] _params = new string[10];
            int offset = 0; // keep track of param
            int i = 0;
            foreach (string value in _values)
            {
                if(value[0]==App.sqlStringValueMarker)
                {
                    _params[offset] = value.Substring(1); // remove marker
                    _values[i] = $"@{offset}";
                    offset++; // only increment when sql found
                }
                i++;    // loop counter
            }
            int maxOffset = offset;
            // oh balls would need to convert them back to their original data types so sql can convert them back
            // I'm only worried about strings, so I'll use placeholders only for strings
            // in most cases, I'm trying to encode SQL queries!!
            // maybe use a special syntax such as ^ to denote an sql query string
            // now use the values with params - use ordinals as placeholders
            
            
            
            // need to rejoin them!! this is simply for compat with update
            values = String.Join(",", _values); // includes placeholders
            
            //values = values.Replace("'", "\""); // escape

            string sql = $"INSERT INTO [{tableName}] ({fields}) VALUES ({values})";
            if (!connected) connect();

            timing.start();
            // escaping single quotes
            //sql = sql.Replace("'", "'''");  // 3 '

            cmd = new OleDbCommand(sql, conn);
            offset = 0;
            for(offset = 0;offset < maxOffset;offset++)
            {
                cmd.Parameters.AddWithValue($"@{offset}", _params[offset]);

            }
            if (hasSql)
            {

                cmd.Parameters.AddWithValue($"@sql", sqlQuery);
            }

            rowsAffected = cmd.ExecuteNonQuery();
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
