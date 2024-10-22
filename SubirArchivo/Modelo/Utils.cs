using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubirArchivo.Entidades
{
    public  class Utils
    {
        public static long ToUnixTime(DateTime date)
        {
            DateTimeOffset offset = new DateTimeOffset(date.ToUniversalTime());
            return offset.ToUnixTimeMilliseconds();
        }
        public static DateTime FromUnixTime(long unixTime)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(unixTime);
            return dateTimeOffset.UtcDateTime.AddHours(-5);
        }
    }
}
