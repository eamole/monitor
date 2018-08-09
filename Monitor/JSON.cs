using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    class JSON
    {
        public static string encode(Dictionary<string, object> obj)
        {
            string json = "{ \n";
            int i = 0;
            foreach (KeyValuePair<string, object> kv in obj)
            {
                if (i++ > 0) json += ",\n";
                json += $"\t \"{kv.Key}\" : " + encodeValue(kv.Value);
            }
            json += "\n}\n";

            return json;

        }

        public static string encodeValue(object value)
        {
            string json = "";
            if (value is string) json = $"\"{value}\"";
            else if (Data.IsNumeric(value)) json = $"\"{value}\"";
            else if (value is bool) json = (bool)value ? "true" : "false";
            else if (value is DateTime) json = ((DateTime)value).ToString(App.dateTimeMask);
            else throw new Exception("Haven't yet encoded objects");
            return json;
        }
    }
}
