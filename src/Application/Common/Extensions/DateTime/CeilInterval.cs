using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Extensions
{
    public static partial class DateTimeExtensions
    {
        public static DateTime CeilYear(this DateTime source)
        {
            var d = source.FloorMonth();
            d = d.AddMonths(-d.Month + 1);
            return d.AddYears(1);
        }

        public static DateTime CeilMonth(this DateTime source)
        {
            var d = source.FloorDay();
            d = d.AddDays(-d.Day + 1);
            return d.AddMonths(1);
        }

        public static DateTime CeilDay(this DateTime source)
        {
            var d = source.FloorHour();
            d = d.AddHours(-d.Hour);
            return d.AddDays(1);
        }

        public static DateTime CeilHour(this DateTime source)
        {
            var d = source.AddMilliseconds(-source.Millisecond);
            d = d.AddSeconds(-source.Second);
            d = d.AddMinutes(-source.Minute);
            d = d.AddHours(1);
            return d;
        }
    }
}