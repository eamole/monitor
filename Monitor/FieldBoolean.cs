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
            if (hasDefault) defaultValue = bool.Parse(defaultValueString);
          
        }


        public FieldBoolean(Table t, OleDbDataReader reader) : base(t, reader)
        {
            if (hasDefault) defaultValue = bool.Parse(defaultValueString);

        }
    }
}
