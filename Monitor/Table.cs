using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

/*

** Error adding _id to Diary : Resultant table not allowed to have more than one AutoNumber field.
** Error adding _id to Hold_Bookings : Primary key already exists.
** Error adding _id to invoice_agents : Primary key already exists.
** Error adding _id to Item_ProductID : Primary key already exists.
** Error adding _id to Product_Type_Types : Primary key already exists.
** Error adding _id to product_types : Primary key already exists.
** Error adding _id to products : Primary key already exists.
** Error adding _id to Report_Agents : Primary key already exists.
** Error adding _id to Report_Bookings : Primary key already exists.
** Error adding _id to report_product_types : Primary key already exists.
 
*/

namespace Monitor   // possibly should be Db
{
    class Table
    {
        public Db db;
        public int id;
        public int dbId;

        public string name;
        public DateTime created;
        public DateTime updated;
        public int recCount;
        // TODO : need the ID as well - should maybe create an FieldList object with ID and the actual list
        public string[] fieldLists;
        public int[] fieldListIds;  // hack


        public int insertedRecordCount;  // a storage area for a recCount when checking for new recrods

        public Dictionary<string, Field> fields;
        
        public Table(Db _db, string _name)
        {
            db = _db;
            name = _name;
            fields = new Dictionary<string, Field>();

            db.tables.Add(name, this);
        }


        // fromSnapshot
        public static Table getTable(OleDbDataReader reader)
        {
            Snapshot db = App.snapshot;
            string tableName = reader.GetValue(reader.GetOrdinal("name")).ToString();

            Table table = null;
            if (db.tables.ContainsKey(tableName))
                table = db.tables[tableName];
            else
                table = new Table(db, tableName);

            table.read(reader);

            table.snapshotGetFields();
            // get the fields

            return table;
        }

        public bool getLastUpdate()
        {
            DataTable schema = App.originalDb.getTables(name);
            DateTime lastUpdate = DateTime.Parse(schema.Rows[0]["DATE_MODIFIED"].ToString());
            bool isUpdated = false;
            TimeSpan t = lastUpdate - updated;
            if(!lastUpdate.Equals(updated))
            {
                App.log($"Table changed (last remembered update : {updated} | latest update ({lastUpdate}) ");
                updated = lastUpdate;
                isUpdated = true;
                save(false);
            } else
            {
                App.log("no changes based on table lupdate date");
            }
            return isUpdated;

        }

        //  moved to Db
        public static Table fromAdoDb(DataRow tableMeta)
        {
            Db db = App.originalDb;
            string tableName = tableMeta["TABLE_NAME"].ToString();
            Table table = new Table(db, tableName);
            table.created = DateTime.Parse(tableMeta["DATE_CREATED"].ToString());
            table.updated = DateTime.Parse(tableMeta["DATE_MODIFIED"].ToString());

            table.getRecCount();
            table.adodbGetFields();

            return table;
        }
        /*
         * return fields as a data source
         */
        public static DataTable getFields(string tableName)
        {
            Snapshot db = App.snapshot;
            Table table = db.tables[tableName];
            OleDbDataReader reader = db.query("fields", $" [tableId] = {table.id}", "ordinal");
            OleDbDataAdapter da = new OleDbDataAdapter(db.cmd);
            DataTable _fields = new DataTable();

            reader.Close();

            da.Fill(_fields);
            return _fields;
        }

        public static DataTable getData(Snapshot snapshot, string tableName)
        {
            
            Db db = snapshot.originalDb;
            Table table = snapshot.tables[tableName];
            OleDbDataReader reader = db.query($"[{tableName}]");
            OleDbDataAdapter da = new OleDbDataAdapter(db.cmd);
            DataTable data = new DataTable();

            reader.Close();

            da.Fill(data);
            return data;

        }

        public static DataTable getFieldStats(string tableName)
        {
            Snapshot db = App.snapshot;
            Table table = db.tables[tableName];
            int batchId = db.getLastBatchId();
            //reader = db.query("fieldStats", $" [tableId] = {table.id} AND [batchId] = {batchId}");
            string fields = "fields.tableName, fields.name, fieldStats.dateTime, fieldStats.nulls, fieldStats.recCount";
            OleDbDataReader reader = db.join(fields, "fields,fieldStats", "fields.id = fieldStats.fieldId", $" [fields.tableId] = {table.id} AND [fieldStats.batchId] = {batchId}","fields.ordinal");


            OleDbDataAdapter da = new OleDbDataAdapter(db.cmd);
            DataTable _fields = new DataTable();

            reader.Close();

            da.Fill(_fields);
            return _fields;

        }




        public void snapshotGetFields()
        {
            OleDbDataReader reader = db.query("fields", $" [tableId] = {id}", "ordinal");
            while(reader.Read())
            {
                Field.fromSnapshot(this, reader);
            }
            reader.Close();
        }


        public void insertIdField(Snapshot snapshot)
        {
         
            try
            {
                // 
                string sql = $"ALTER TABLE [{name}] ADD COLUMN [_id] AutoIncrement PRIMARY KEY; ";  // 
                snapshot.originalDb.sql(sql);

            } catch(Exception e)
            {
                Console.WriteLine($"** Error adding _id to {name} : {e.Message}");
            }

        }

        public static void insertIdField(Snapshot snapshot, string tableName)
        {
            Db db = snapshot.originalDb;

            // compute a JSON version of the record?
            Table table = snapshot.tables[tableName];
            string sql = $"ALTER TABLE [{tableName}] ADD COLUMN [id] AutoIncrement PRIMARY KEY; ";  // 
            db.sql(sql);

        }

        public static void deleteIdField(Snapshot snapshot, string tableName)
        {
            Db db = snapshot.originalDb;

            // compute a JSON version of the record?
            Table table = snapshot.tables[tableName];
            string sql = $"ALTER TABLE [{tableName}] DROP COLUMN [id]";  // 
            db.sql(sql);

        }

        public static void genFieldLists(string tableName)
        {
            Db db = App.snapshot.originalDb;

            Table table = App.snapshot.tables[tableName];
            int batchId = App.snapshot.getLastBatchId();
            // I need to view the fields in nulls increasing order - need a join with fieldStats
            string sql = $@"SELECT fields.*, fieldStats.nulls, 
                            fieldStats.recCount, fieldStats.batchId
                            FROM fields 
                                INNER JOIN fieldStats ON fields.id = fieldStats.fieldId
                            WHERE (fieldStats.batchId={batchId}) AND ([fields.tableId] = {table.id})
                            ORDER BY fieldStats.nulls ASC ;";

            int fieldList = 1;
            int i = 0;

            OleDbDataReader reader = App.snapshot.sql(sql);
            string delim = "\"";   // use a special char as delim
            sql = delim;
            bool written=false;
            while (reader.Read())
            {
                written = false;
                Field field = Field.fromSnapshot(table, reader);
                field.fieldList = fieldList;

                if (i++ > 0) sql += $" & {App.fieldListSeparator} & ";  // 
                                                                        // if there are no records, return 0 not Null
                                                                        //sql += $"'{field.name} :' ";
                if (field.isString)
                    sql += $" [{field.name}] ";
                if (field.isNumber)
                    sql += $" Str( [{field.name}] )";
                if (field.isDate)
                    sql += $" Format([{field.name}],'yyyy-mm-dd hh:mm:ss') ";   // ' causes problems in expr string
                if (field.isBool)
                    sql += $" [{field.name}] ";
                if (i == App.maxFieldListSize)
                {
                    // break the list
                    // write the field list
                    sql += delim;
                    App.snapshot.upsert("fieldLists", "tableId,ordinal,fieldList",
                        $"|{table.id}|{fieldList}|{sql}",
                        $"[tableId]={table.id} AND [ordinal]={fieldList}");

                    written = true;
                    sql = delim;
                    i = 0;
                    fieldList++;
                }
            }

            if (!written)
            {     // write last fieldList 
                sql += delim;
                App.snapshot.upsert("fieldLists", "tableId,ordinal,fieldList",
                $"|{table.id}|{fieldList}|{sql}",
                $"[tableId]={table.id} AND [ordinal]={fieldList}");
            }
            reader.Close();
            table.save();
            
        }

        public string[] getFieldLists()
        {
            Snapshot db = App.snapshot;
            int count = db.recCount("fieldLists", $"[tableId]={id}");
            fieldLists = new string[count];
            fieldListIds = new int[count];
            OleDbDataReader reader = db.query("fieldLists", $"[tableId]={id}", "ordinal");
            int i = 0;
            while (reader.Read())
            {
                fieldLists[i] = reader.GetValue(reader.GetOrdinal("fieldList")).ToString();
                fieldListIds[i++] = int.Parse(reader.GetValue(reader.GetOrdinal("id")).ToString());
            }
            return fieldLists;
        }
        // no longer used
        public static string getFieldExpression(string tableName)
        {
            Snapshot snapshot = App.snapshot;
            Db db = snapshot.originalDb;

            // compute a JSON version of the record?
            Table table = snapshot.tables[tableName];

            string sql = "";
            int i = 0;
            foreach (KeyValuePair<string, Field> entry in table.fields)
            {
                Field field = entry.Value;
                if (i++ > 0) sql += $" & {App.fieldListSeparator} & ";  // 
                                                       // if there are no records, return 0 not Null
                                                       //sql += $"'{field.name} :' ";
                if (field.isString)
                    sql += $" [{field.name}] ";
                if (field.isNumber)
                    sql += $" Str( [{field.name}] )";
                if (field.isDate)
                    sql += $" Format([{field.name}],'yyyy-mm-dd hh:mm:ss') ";
                if (field.isBool)
                    sql += $" [{field.name}] ";
                if (i == App.maxFieldListSize) break;
            }
            Console.WriteLine(sql);
            return sql;
        }

        public static void takeDataSnapshot()
        {
            /*
             * inserts
             * look for id in 
             * compute a String version of the record?
             */ 
        } 

        public int countNewRecords() {
            App.log("Getting new record count for " + name);
            string sql = $@"SELECT Count(*) as RecCount FROM [{name}]
                                WHERE [{name}].[_id] NOT IN 
                                   (SELECT [_id] FROM [snapshot] WHERE [tableId] = {id} );";

            OleDbDataReader reader = App.snapshot.sql(sql);
            reader.Read();
            int count = int.Parse(reader.GetValue(reader.GetOrdinal("RecCount")).ToString());
            App.log("done. Count : " + count);
            return count;
        }

        // will return a string with either the ids in an array of a table in clause
        public string getNewRecordIdsClause()
        {
            string clause = "";
            int count = countNewRecords();
            insertedRecordCount = count;  // so it can be used outside
            if (count == 0)
            {
                return clause;
            }
            if(count>App.maxIdArraySize)
            {
                clause = getNewRecordIdsTable(count);

            } else
            {
                int[] _ids = getNewRecordIdsArray(count);
                if (_ids == null)
                {
                    Console.WriteLine($"Check for Inserts - no NEW records on table {name}");
                    return clause;
                }
                string ids = String.Join(",", _ids);

                clause = $" ({ids}) ";
                
            }
            return clause;
        }
        public string getNewRecordIdsTable(int count)
        {

            string sql = $@"INSERT INTO tempIds ([_id]) 
                               SELECT [_id] AS ID FROM [{name}]
                                 WHERE [_id] NOT IN 
                                   (SELECT [_id] FROM [snapshot] WHERE [tableId] = {id} );";

            // clear temp file
            App.snapshot.delete("tempIds", "", true);
            App.snapshot.sql(sql);
            string clause = " (SELECT [_id] FROM TempIds) ";
            return clause;
        }

        public int[] getNewRecordIdsArray(int count)
        {

            int[] ids = null;
            if(count==0)
            {
                return ids;
            }

            string sql = $@"SELECT [_id] AS ID FROM [{name}]
                                WHERE [_id] NOT IN 
                                   (SELECT [_id] FROM [snapshot] WHERE [tableId] = {id} );";

            OleDbDataReader reader = App.snapshot.sql(sql);
            if (reader.HasRows)
            {
                reader.Read();
                ids = new int[count];
                int i = 0;
                do
                {
                    ids[i++] = int.Parse(reader.GetValue(reader.GetOrdinal("ID")).ToString());
                } while (reader.Read());

            }
            return ids;
        }
        /*
         * change so that snapshotValues holds the actual data - 1 rec pre fieldlist
         * this makes updates eaaier to detect
         */
        public static void checkForInserts(string tableName) 
        {

            Db db = App.snapshot.originalDb;
            Table table = App.snapshot.tables[tableName];
            string op = "i";
            //int count = db.recCount($"{table.name}");

            table.getRecCount();

            // an unchanged record count not sufficient. 
            if(table.recCount == 0)
            {
                App.log($"Check for Inserts - no records on table {table.name}");
                return;
            }
            // check last update v last scan - don't scan if no change since last scan
            //if(!table.getLastUpdate())
            //{
            //    App.log($"Check for Inserts - no updates based on date {table.name}");
            //    return;
            //}
            // if no change



            if (table.fieldLists==null)
                table.getFieldLists();

            int statsId = App.snapshot.getNewStats(op, table.id);

            string inClause = table.getNewRecordIdsClause();
            if (table.insertedRecordCount == 0)   // set by getNewRecordCount 
            {
                Console.WriteLine($"Check for Inserts - no NEW records on table {table.name}");
                return;
            }
            // we have new inserts - get the expressions
            int i = 0;
            string sql = "";
            foreach (string expr in table.fieldLists)
            {
                int fieldListId = table.fieldListIds[i];
                // 2 - use a snapshotValues table - and insert into a join - record per fieldlist
                // can't insert into a JOIN, but by updating, can auto create records!!
                // UPDATE snapshot 
                // INNER JOIN snapshotValues ON snapshotValues.snapshotId = snapshotId
                sql = $@"INSERT INTO qrySnapshotJoin 
                            ([_id],snapshotValues.op,[tableId],[statsId],[fieldlistId],[ordinal],[current])
                            SELECT [_id],'{op}',{table.id},{statsId},{fieldListId},{i+1},{expr} AS expr FROM [{tableName}] 
                                WHERE [_id] IN {inClause};";


                //Console.WriteLine(sql);
                try
                {
                    //OleDbDataReader reader = 
                    App.snapshot.sql(sql);
                    // how many records added? - actually second + passes are techincally updates
                    // and we know the record count - we got it from _ids
                    //int newRecords = App.snapshot.recCount("snapshot", $"[statsId]={statsId}");
                    App.snapshot.recordStats(statsId, table.insertedRecordCount);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error " + e.Message);
                    App.log(sql);
                }

                i++;    //next field list
            }

        }

        public string computeFieldStatsQuery()
        {
            string sql = "SELECT ";
            int i=0;
            foreach (KeyValuePair<string,Field> entry in fields)
            {
                if (i++ > 0) sql += ","; 
                Field field = entry.Value;
                // if there are no records, return 0 not Null
                string extra="0";

                if (field.isString)
                    extra = $"IIF([{field.name}]='',1,0)";
                if (field.isNumber)
                    extra = $"IIF([{field.name}]=0,1,{extra})";
                if (field.isDate && field.hasDefault)
                    extra = $"IIF([{field.name}]=#{field.defaultValueString}#,1,{extra})";

                sql += $"Sum( IIf( [{field.name}] IS NULL , 1, {extra} )) AS [nulls_{field.name}] ";
                
                //sql += $"Sum( IsNull(NullIf([{field.name}],'')) AS [nulls_{field.name}] ";

            }
            sql += $" FROM [{name}] ;";
            return sql;
        }

        public void readerDump(OleDbDataReader reader)
        {
            while(reader.Read())
            {
                for(int i=0;i<reader.FieldCount;i++)
                {
                    Console.Write(reader.GetName(i), ":");
                    Console.WriteLine(reader[i].ToString());
                }
            }
        }
        
        
        
        
        
        /*
         * reader is null if there are no records - write 0's
         */

        public void writeFieldStats(int batchId ,OleDbDataReader reader=null)
        {
            string sql = "";
            //if (reader != null)
            //    readerDump(reader);
            if (reader !=null) 
                reader.Read();
            //sql = $"INSERT INTO [fieldStats] ([batchId],[fieldId],[nulls],[recCount]) VALUES ";
            int i = 0;
            foreach (KeyValuePair<string, Field> entry in fields)
            {
                if (i++ > 0) sql += ",";    // between values
                Field field = entry.Value;

                string fieldName = $"nulls_{field.name}";   // no brackets in c# dict lookup

                int nulls = 0;
                if(reader != null)
                    nulls = int.Parse(reader.GetValue(reader.GetOrdinal(fieldName)).ToString());

                sql = $"INSERT INTO [fieldStats] ([batchId],[fieldId],[nulls],[recCount]) VALUES ";
                sql += $"({batchId},{field.id},{nulls},{recCount}) ";
                // have to run one insert at a time!!
                db.sql(sql);
            }
            // multiple queries not allowed
            // mutiple values not allowed!!
            //sql += ";";
            //db.sql(sql);
            Console.WriteLine("Done");
        }

        public bool getRecCount()
        {
            int count = App.originalDb.recCount(name);
            bool isUpdated = false;
            if (count != recCount)
            {
                Console.WriteLine($"Table {name} | changed recCount | old : {recCount} | new : {count} ");
                recCount = count;
                isUpdated = true;
                save(false);
            }

            return isUpdated;
        }
        public void adodbGetFields()
        {
            DataTable _fields = App.originalDb.getFields(name);
            foreach(DataRow row in _fields.Rows)
            {
                // each row should be a field
                Field field = Field.fromAdoDb(this, row);
                Console.WriteLine("Column : " + field.name);
                foreach(DataColumn col in _fields.Columns)
                {
                    object cell = row[col];
                    Console.WriteLine($" {col.ColumnName} : \"{cell}\"");
                }

            }
        }

        public void save(bool saveFields=true)
        {
            // insert into tables if not exists
            string fields = "name,created,updated,recCount";
            string values = $"\"{name}\" , #{created}# , #{updated}# , {recCount}";
            if (id>0)
            {
                // this is a bound object
                db.update("tables", fields, values, $" id = {id}");

            } else
            {
                // see if the record exists
                Table table = load(db, name);
                if(table==null)
                {
                    // record does not exist
                    db.insert("tables", fields, values);
                } else
                {
                    // need to update
                    db.update("tables", fields, values, $" name = \"{name}\"");
                    // get the ID
                }
                // load the table object into this object
                table = load(db, name, this);

            }
            if(saveFields)
            {
                foreach (KeyValuePair<string, Field> entry in this.fields)
                {
                    Field field = entry.Value;
                    field.save();
                }
            }

        }
        public void read(OleDbDataReader reader)
        {
            id = int.Parse(reader.GetValue(reader.GetOrdinal("id")).ToString());
            created = DateTime.Parse(reader.GetValue(reader.GetOrdinal("created")).ToString());
            updated = DateTime.Parse(reader.GetValue(reader.GetOrdinal("updated")).ToString());
            recCount = int.Parse(reader.GetValue(reader.GetOrdinal("recCount")).ToString());
        }
        public Table load(Db db, string tableName,Table table = null)
        {
            OleDbDataReader reader = db.query("tables", $" name = \"{tableName}\" ");
            if(reader.HasRows)
            {
                if(table == null)
                    table = new Table(db, tableName);

                while (reader.Read())
                {
                    table.read(reader);
                }
                reader.Close();
            }
            return table;
        }

     }
 }
