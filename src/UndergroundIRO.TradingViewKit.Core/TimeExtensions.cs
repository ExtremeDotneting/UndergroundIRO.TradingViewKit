using System;

namespace UndergroundIRO.TradingViewKit.Core
{
    public static class TimeExtensions
    {
        public static double ToUniversalDateTime(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public static DateTime FromUniversalDateTime(double milliseconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds).ToLocalTime();
        }
    }
}
