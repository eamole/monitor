using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace Monitor
{
    class FieldBoolean : Field
    {
        public bool defaultValue; // if it has one
        
        // should add the collation and char set info

        public FieldBoolean(Table t, DataRow row) : base(t, row)
        {
            isBool = true;
            if (hasDefault) defaultValue = bool.Parse(defaultValueString);
          
        }


        public FieldBoolean(Table t, OleDbDataReader reader) : base(t, reader)
        {
            isBool = true;
            if (hasDefault) defaultValue = bool.Parse(defaultValueString);

        }

        override public object parseSqlValue(string sqlValue)
        {
            //            if (sqlValue.Length == 0) return false;
            if (sqlValue == "0") return false;
            else if (sqlValue == "-1") return true;
            else throw new Exception($"Invalid sql boolean type value {sqlValue}");
            return bool.Parse(sqlValue);
        }

    }
}
