using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QRESTModel.BLL
{
    public static class UtilsText
    {
        /// <summary>
        ///  Generic data type converter 
        /// </summary>
        private static bool TryConvert<T>(object value, out T result)
        {
            result = default(T);
            if (value == null || value == DBNull.Value) return false;

            if (typeof(T) == value.GetType())
            {
                result = (T)value;
                return true;
            }

            string typeName = typeof(T).Name;

            try
            {
                if (typeName.IndexOf(typeof(System.Nullable).Name, StringComparison.Ordinal) > -1 ||
                    typeof(T).BaseType.Name.IndexOf(typeof(System.Enum).Name, StringComparison.Ordinal) > -1)
                {
                    TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));
                    result = (T)tc.ConvertFrom(value);
                }
                else
                    result = (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return false;
            }

            return true;
        }


        /// <summary>
        ///  Converts to another datatype or returns default value
        /// </summary>
        public static T ConvertOrDefault<T>(this object value)
        {
            TryConvert<T>(value, out T result);
            return result;
        }


        /// <summary>
        ///  Better than built-in SubString by handling cases where string is too short
        /// </summary>
        /// <param name="index">Zero based</param>
        public static string SubStringPlus(this string str, int index, int length)
        {
            return str?.Substring(index, Math.Min(str.Length - index, length));
        }


        public static bool IsNumeric(this String s) {
            return decimal.TryParse(s, out _);
        }



        public static string[] GetDateTimeAllowedFormats(string dtFormat, string tmFormat)
        {
            //date format can be "MM/dd/yyyy", or "yy/MM/dd"
            //time format can be "HH:MM", "HH:mm:ss"

            //date handling
            string[] dts = new[] { "MM/dd/yyyy", "M/dd/yyyy", "MM/d/yyyy", "M/d/yyyy" };
            if (dtFormat == "yy/MM/dd")
                dts = new[] { "yy/MM/dd", "yy/M/dd", "yy/M/d", "yy/MM/d" };

            string[] tms = new[] { "HH:mm", "H:mm" };
            if (tmFormat == "HH:mm:ss")
                tms = new[] { "HH:mm:ss", "H:mm:ss" };

            List<string> combo = new List<string>();

            foreach (string x in dts)
                foreach (string y in tms)
                    combo.Add(x + " " + y);

            return combo.ToArray();
        }



        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
