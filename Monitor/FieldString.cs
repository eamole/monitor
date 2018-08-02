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

        }

        public FieldString(Table t, OleDbDataReader reader) : base(t, reader)
        {
            if (hasDefault) defaultValue = defaultValueString;
            maxLength = int.Parse(reader.GetValue(reader.GetOrdinal("maxLength")).ToString());

        }

    }
}
