﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace Monitor
{
    class FieldDateTime : Field
    {
        public DateTime defaultValue;   // might be a function - now()

        public FieldDateTime(Table t, DataRow row) : base(t, row)
        {
            isDate = true;
            if (hasDefault)
            {
                if(defaultValueString == "=Now()")
                {
                    // don't do anything?
                } else
                {
                    try
                    {
                        defaultValue = DateTime.Parse(defaultValueString);
                    } catch(Exception e)
                    {
                        Console.WriteLine($"DateTime Parse Error : {defaultValueString } | Table : {table.name} | Field : {name}");
                    }
                    
                }
                
            }
        }

        public FieldDateTime(Table t, OleDbDataReader reader) : base(t, reader)
        {

            isDate = true;
            if (hasDefault)
            {
                if (defaultValueString == "=Now()")
                {
                    // don't do anything?
                }
                else
                {
                    try
                    {
                        defaultValue = DateTime.Parse(defaultValueString);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"DateTime Parse Error : {defaultValueString } | Table : {table.name} | Field : {name}");
                    }

                }

            }
        }

        override public object parseSqlValue(string sqlValue)
        {
            //if (sqlValue.Length == 0) return sqlValue;  // what is the default date???
            return DateTime.Parse(sqlValue);    // need to be careful
        }

    }
}
