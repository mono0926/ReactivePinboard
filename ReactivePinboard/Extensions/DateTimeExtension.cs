using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mono.Api.ReactivePinboard.Extensions
{
    internal static class DateTimeExtension
    {
        public static string ToUTCDateStr(this DateTime date)
        {
            return TimeZoneInfo.ConvertTime(date, TimeZoneInfo.Utc).ToString("yyyy-MM-dd");
        }

        public static string ToUTCDateTimeStr(this DateTime date)
        {
            return TimeZoneInfo.ConvertTime(date, TimeZoneInfo.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ");
        }
    }
}