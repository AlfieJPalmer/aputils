using System;
using System.Collections.Generic;

namespace testproj
{
    public static class Extensions
    {
        public static string RemoveDataTypeSuffix(this object o)
        {
            var suffixChars = new List<char> { 'f', 'd', 'l', 'o' };
            string objStr = o.ToTrimString().ToLower();

            foreach (var suffix in suffixChars)
            {
                if (!objStr.EndsWith(suffix.ToString()))
                    continue;

                objStr = objStr.TrimEnd(suffix);
                return objStr;
            }
            return objStr;
        }

        public static string ToTrimString(this object o)
        {
            return o?.ToString().Trim() ?? "";
        }

        public static int ToInt(this object o)
        {
            int ret;
            return int.TryParse(o.RemoveDataTypeSuffix(), out ret) ? ret : (int)Math.Floor(ToDouble(o));
        }

        public static long ToLong(this object o)
        {
            long ret;
            return long.TryParse(o.RemoveDataTypeSuffix(), out ret) ? ret : (long)Math.Floor(ToDouble(o));
        }

        public static float ToFloat(this object o)
        {
            float ret;
            return float.TryParse(o.RemoveDataTypeSuffix(), out ret) ? ret : float.NaN;
        }

        public static double ToDouble(this object o)
        {
            double ret;
            return double.TryParse(o.RemoveDataTypeSuffix(), out ret) ? ret : double.NaN;
        }

        public static decimal ToDecimal(this object o)
        {
            decimal ret;
            return decimal.TryParse(o.RemoveDataTypeSuffix(), out ret) ? ret : decimal.MinValue;
        }

        public static bool ToBoolean(this object o)
        {
            if (o.ToInt() == 0) return false;
            if (o.ToInt() >= 1) return true;

            bool ret;
            return bool.TryParse(o.ToTrimString(), out ret) && ret;
        }
        
        public static DateTime ToDateTime(this object obj)
        {
            if (obj is DateTime)
                return (DateTime)obj;

            double objDbl;
            if (double.TryParse(obj.ToString(), out objDbl))
                return DateTime.FromOADate(objDbl);
            
            DateTime ret;
            return DateTime.TryParse(obj.ToTrimString(), out ret) ? ret : DateTime.MinValue;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {

        }
    }
}
