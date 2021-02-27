using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Extensions
{
    public static partial class DateTimeExtensions
    {
        public static DateTime FloorYear(this DateTime source)
        {
            var d = source.FloorMonth();
            return d.AddMonths(-source.Month + 1);
        }

        public static DateTime FloorMonth(this DateTime source)
        {
            var d = source.FloorDay();
            return d.AddDays(-source.Day + 1);
        }

        public static DateTime FloorDay(this DateTime source)
        {
            var d = source.FloorHour();
            return d.AddHours(-source.Hour);
        }

        public static DateTime FloorHour(this DateTime source)
        {
            var d = source.AddMilliseconds(-source.Millisecond);
            d = d.AddSeconds(-source.Second);
            d = d.AddMinutes(-source.Minute);
            return d;
        }
    }
}