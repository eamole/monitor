using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace Monitor
{
    class FieldString : Field
    {
        public string defaultValue; // if it has one
        public int maxLength;
        // should add the collation and char set info

        public FieldString(Table t,  DataRow row) : base(t, row)
        {
            if (hasDefault) defaultValue = defaultValueString;
            maxLength = int.Parse(row["CHARACTER_MAXIMUM_LENGTH"].ToString());
            isString = true;
        }

        public FieldString(Table t, OleDbDataReader reader) : base(t, reader)
        {
            if (hasDefault) defaultValue = defaultValueString;
            maxLength = int.Parse(reader.GetValue(reader.GetOrdinal("maxLength")).ToString());
            isString = true;
        }

        /*
        * this will cause problems based on whether int or float
        */
        override public object parseSqlValue(string sqlValue)
        {
            string value = sqlValue.Trim();
            if (sqlValue.Length == 0) return sqlValue;
            if(value[0]=='\'') value = value.Substring(1);
            if (value.EndsWith("'")) value = value.Substring(0, value.Length - 1);
            return value;    // may need trim off '
        }

    }
}
