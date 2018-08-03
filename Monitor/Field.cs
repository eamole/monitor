using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace Monitor
{
    class Field
    {
        public int id;
        public int tableId;

        public Table table;
        public string tableName;
        public string name;
        public int dataType;
        public int ordinal; // order
        public int flags;
        public bool hasDefault;
        public string defaultValueString;   // the raw data
        public bool isNullable;
        public string description;

        public bool isString=false;
        public bool isNumber = false;
        public bool isDate = false;
        public bool isBool = false;

        public int fieldList = 1;

        public Field(Table _table,DataRow row)
        {
            table = _table;

            tableName = row["TABLE_NAME"].ToString();
            name = row["COLUMN_NAME"].ToString();
            dataType = int.Parse(row["DATA_TYPE"].ToString());
            ordinal = int.Parse(row["ORDINAL_POSITION"].ToString());
            flags = int.Parse(row["COLUMN_FLAGS"].ToString());
            hasDefault = bool.Parse(row["COLUMN_HASDEFAULT"].ToString());
            defaultValueString = row["COLUMN_DEFAULT"].ToString();
            isNullable = bool.Parse(row["IS_NULLABLE"].ToString());
            description = row["DESCRIPTION"].ToString();

        }

        public Field(Table _table, OleDbDataReader reader)
        {
            table = _table;
            read(reader);
            
        }


        public static Field fromSnapshot(Table table,OleDbDataReader reader)
        {
            string name = reader.GetValue(reader.GetOrdinal("name")).ToString();
            int dataType = int.Parse(reader.GetValue(reader.GetOrdinal("dataType")).ToString());
            Field field = null;
            if (table.fields.ContainsKey(name)) field = table.fields[name];
            switch (dataType)
            {
                case (int)OleDbType.BigInt:
                case (int)OleDbType.Decimal:
                case (int)OleDbType.Currency:
                case (int)OleDbType.Double:
                case (int)OleDbType.Integer:
                case (int)OleDbType.Numeric:
                case (int)OleDbType.Single:
                case (int)OleDbType.SmallInt:
                case (int)OleDbType.UnsignedBigInt:
                case (int)OleDbType.UnsignedInt:
                case (int)OleDbType.UnsignedSmallInt:
                case (int)OleDbType.UnsignedTinyInt:
                    {
                        if (field == null)
                            field = new FieldNumeric(table, reader);
                        else
                            field.read(reader);
                        break;
                    }

                case (int)OleDbType.Date:
                case (int)OleDbType.DBDate:
                case (int)OleDbType.DBTime:
                case (int)OleDbType.DBTimeStamp:
                case (int)OleDbType.Filetime:
                    {
                        if (field == null)
                            field = new FieldDateTime(table, reader);
                        else
                            field.read(reader);
                        break;
                    }
                case (int)OleDbType.BSTR:
                case (int)OleDbType.Char:
                case (int)OleDbType.LongVarChar:
                case (int)OleDbType.LongVarWChar:
                case (int)OleDbType.VarChar:
                case (int)OleDbType.VarWChar:
                case (int)OleDbType.WChar:
                    {
                        if (field == null)
                            field = new FieldString(table, reader);
                        else
                            field.read(reader);
                        break;
                    }
                case (int)OleDbType.Boolean:
                    {
                        if (field == null)
                            field = new FieldBoolean(table, reader);
                        else
                            field.read(reader);
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"Error : Unknown OleDb Data Type {dataType} | Table : {table.name} | Field : {name}");
                        break;
                    }

            }

            // don't add multiple
            if (field != null && !table.fields.ContainsKey(field.name))
                table.fields.Add(field.name, field);

            return field;
        }
    
        public static Field fromAdoDb(Table table,DataRow row)
        {
            string name = row["COLUMN_NAME"].ToString();
            int dataType = int.Parse(row["DATA_TYPE"].ToString());
            Field field = null;
            switch(dataType)
            {
                case (int)OleDbType.BigInt:
                case (int)OleDbType.Decimal:
                case (int)OleDbType.Currency:
                case (int)OleDbType.Double:
                case (int)OleDbType.Integer:
                case (int)OleDbType.Numeric:
                case (int)OleDbType.Single:
                case (int)OleDbType.SmallInt:
                case (int)OleDbType.UnsignedBigInt:
                case (int)OleDbType.UnsignedInt:
                case (int)OleDbType.UnsignedSmallInt:
                case (int)OleDbType.UnsignedTinyInt:
                    {
                        field = new FieldNumeric(table, row);
                        break;
                    }

                case (int)OleDbType.Date:
                case (int)OleDbType.DBDate:
                case (int)OleDbType.DBTime:
                case (int)OleDbType.DBTimeStamp:
                case (int)OleDbType.Filetime:
                    {
                        field = new FieldDateTime(table, row);
                        break;
                    }
                case (int)OleDbType.BSTR:
                case (int)OleDbType.Char:
                case (int)OleDbType.LongVarChar:
                case (int)OleDbType.LongVarWChar:
                case (int)OleDbType.VarChar:
                case (int)OleDbType.VarWChar:
                case (int)OleDbType.WChar:
                    {
                        field = new FieldString(table, row);
                        break;
                    }
                case (int)OleDbType.Boolean:
                    {
                        field = new FieldBoolean(table, row);
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"Error : Unknown OleDb Data Type {dataType} | Table : {table.name} | Field : {name}");
                        break;
                    }

            }

            if(field !=null)
                table.fields.Add(field.name, field);

            return field;
        }

        public void save()
        {
            Db db = table.db;

            if(table.id == 0)
            {
                table.save();   // need to save parent first - to get parent.id
            }
            tableId = table.id;
            // insert into tables if not exists
            string fields = "tableId,name,tableName,dataType,ordinal,flags,hasDefault,defaultValue,isNullable,description";
            string values = $"{tableId},'{name}','{tableName}',{dataType},{ordinal},{flags},{hasDefault},'{defaultValueString}',{isNullable},'{description}' ";
            if (id > 0)
            {
                // this is a bound object
                db.update("fields", fields, values, $" id = {id}");

            }
            else
            {
                // see if the record exists
                Field field = load(db, table , name);
                if (field == null)
                {
                    // record does not exist
                    db.insert("fields", fields, values);
                }
                else
                {
                    // need to update - not so sure!! I've just read it!!!
                    db.update("fields", fields, values, $" tableId = {table.id} AND name = '{name}' ");
                    // get the ID
                }
                // load the table object into this object
                field = load(db, table, name, this);

            }
        }
        public void read(OleDbDataReader reader)
        {
            /*
             * jaysus - these all fail on NULLs
             * need a better way of reading records!! Maybe pass an array in of type
             * bind name of field to property of same name
             */
            id = int.Parse(reader.GetValue(reader.GetOrdinal("id")).ToString());
            tableId = int.Parse(reader.GetValue(reader.GetOrdinal("tableId")).ToString());

            tableName = reader.GetValue(reader.GetOrdinal("tableName")).ToString();
            name = reader.GetValue(reader.GetOrdinal("name")).ToString();
            dataType = int.Parse(reader.GetValue(reader.GetOrdinal("dataType")).ToString());
            ordinal = int.Parse(reader.GetValue(reader.GetOrdinal("ordinal")).ToString());
            flags = int.Parse(reader.GetValue(reader.GetOrdinal("flags")).ToString());
            hasDefault = bool.Parse(reader.GetValue(reader.GetOrdinal("hasDefault")).ToString());
            defaultValueString = reader.GetValue(reader.GetOrdinal("defaultValue")).ToString();
            isNullable = bool.Parse(reader.GetValue(reader.GetOrdinal("isNullable")).ToString());
            description = reader.GetValue(reader.GetOrdinal("description")).ToString();

            fieldList = int.Parse(reader.GetValue(reader.GetOrdinal("fieldList")).ToString());

        }
        public Field load(Db db, Table table , string fieldName, Field field = null)
        {
            OleDbDataReader reader = db.query("fields", $" tableId = {table.id} AND name = '{fieldName}' ");
            if (reader.HasRows)
            {
                reader.Read();
                if (field == null)  // create the object
                    field = Field.fromSnapshot(table, reader);
                else            // read the object
                    field.read(reader);
                
                reader.Close();
            }
            return field;
        }
    }
}

    