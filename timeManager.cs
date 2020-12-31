using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision_Automix
{
    class TimeManager
    {
        public static Int64 GetTimestamp()
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = DateTime.Now.ToUniversalTime() - origin;

            double returnMe = (double)Math.Floor(diff.TotalSeconds);
            return (Int64)returnMe;
        }

        public static Int64 GetElapsedFromTimestamp(Int64 timestamp)
        {
            Int64 result = (GetTimestamp() - timestamp);
            if (result < 0) { result = 0; }
            return result;
        }
    }
}
