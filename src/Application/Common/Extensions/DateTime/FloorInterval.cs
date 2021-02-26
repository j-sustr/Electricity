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
            return source.AddMonths(-source.Month);
        }

        public static DateTime FloorMonth(this DateTime source)
        {
            var d = source.FloorDay();
            return source.AddDays(-source.Day);
        }

        public static DateTime FloorDay(this DateTime source)
        {
            var d = source.FloorHour();
            return d.AddHours(source.Hour);
        }

        public static DateTime FloorHour(this DateTime source)
        {
            var d = source.AddMilliseconds(-source.Millisecond);
            return d.AddMinutes(-source.Minute);
        }
    }
}