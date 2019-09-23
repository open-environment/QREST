using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
            if (str != null)
                return str.Substring(index, Math.Min(str.Length - index, length));
            else
                return null;
        }


    }
}
