using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public static class Extensions
    {
        /// <summary>
        /// Restituisce gli n caratteri più a sinistra di una stringa
        /// </summary>
        /// <param name="value">stringa</param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string Left(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return value.Length <= maxLength ? value : value[..maxLength];
        }

        public static string Chop(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var len = value.Length - 1;
            return len > 0 ? value[..len] : value;
        }

        public static string LastChar(this string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            var pos = value.Length - 1;
            return pos > 0 ? value.Substring(pos) : value;
        }
    }
}
