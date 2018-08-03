using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace Monitor
{
    class FieldNumeric : Field 
    {
        public int digits;
        public int decimals;
        public double defaultValue;

        public FieldNumeric(Table t, DataRow row) : base(t,row)
        {
            if(hasDefault) defaultValue = int.Parse(defaultValueString);
            digits = int.Parse(row["NUMERIC_PRECISION"].ToString());
            if (!String.IsNullOrEmpty(row["NUMERIC_SCALE"].ToString()))
            {
                decimals = int.Parse(row["NUMERIC_SCALE"].ToString());
            }
            isNumber = true;
        }

        public FieldNumeric(Table t, OleDbDataReader reader) : base(t, reader)
        {
            if (hasDefault) defaultValue = double.Parse(reader.GetValue(reader.GetOrdinal("defaultValue")).ToString());

            digits = int.Parse(reader.GetValue(reader.GetOrdinal("digits")).ToString());
            decimals = int.Parse(reader.GetValue(reader.GetOrdinal("decimals")).ToString());
            isNumber = true;
        }
    }
}
