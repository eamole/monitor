using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace Monitor   // possibly should be Db
{
    class Table
    {
        public Db db;
        public int id;
        public string name;
        public DateTime created;
        public DateTime updated;
        public int recCount;

        public Dictionary<string, Field> fields;
        
        public Table(Db _db, string _name)
        {
            db = _db;
            name = _name;
            fields = new Dictionary<string, Field>();

        }
        public static Table fromSnapshot(Db db,OleDbDataReader reader)
        {
            string tableName = reader.GetValue(reader.GetOrdinal("name")).ToString();
            Table table = new Table(db, tableName);
            table.read(reader);
            return table;
        }

        public static Table fromAdoDb(Db db,DataRow tableMeta)
        {
            string tableName = tableMeta["TABLE_NAME"].ToString();
            Table table = new Table(db, tableName);
            table.created = DateTime.Parse(tableMeta["DATE_CREATED"].ToString());
            table.updated = DateTime.Parse(tableMeta["DATE_MODIFIED"].ToString());

            table.getRecCount();
            table.getFieldsAdoDb();

            return table;
        }
        public int getRecCount()
        {
            recCount = db.recCount(name);
            return recCount;
        }
        public void getFieldsAdoDb()
        {
            DataTable _fields = db.adodbGetFields(name);
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

        public void save()
        {
            // insert into tables if not exists
            string fields = "name,created,updated,recCount";
            string values = $"\"{name}\" , #{created}# , #{updated}# , {recCount}";
            if (id>0)
            {
                // this is a bound object
                db.update("tables", fields, values, " id = {id}");

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
                    db.update("tables", fields, values, " name = \"{name}\"");
                    // get the ID
                }
                // load the table object into this object
                table = load(db, name, this);

            }

            foreach(KeyValuePair<string,Field> entry in this.fields)
            {
                Field field = entry.Value;
                field.save();

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
