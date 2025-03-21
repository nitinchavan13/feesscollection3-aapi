using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace FeesCollection.BusinessLayer.Utility
{
    public static class TimezoneHelper
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        public static DateTime getLocaltimeFromUniversal(DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, INDIAN_ZONE);
        }
        public static DateTime ConvertLocalToUTCwithTimeZone(DateTime localDateTime)
        {
            localDateTime = DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, INDIAN_ZONE);
        }
    }
}
