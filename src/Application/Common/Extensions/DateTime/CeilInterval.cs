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
            if (d != source)
            {
                d = d.AddYears(1); ;
            }
            return d;
        }

        public static DateTime CeilMonth(this DateTime source)
        {
            var d = source.FloorDay();
            d = d.AddDays(-d.Day + 1);
            if (d != source)
            {
                d = d.AddMonths(1);
            }
            return d;
        }

        public static DateTime CeilWeek(this DateTime source, bool mondayFirst = true)
        {
            var d = source.FloorWeek(mondayFirst);
            if (d != source)
            {
                d = d.AddDays(7);
            }
            return d;
        }

        public static DateTime CeilDay(this DateTime source)
        {
            var d = source.FloorHour();
            d = d.AddHours(-d.Hour);
            if (d != source)
            {
                d = d.AddDays(1);
            }
            return d;
        }

        public static DateTime CeilHour(this DateTime source)
        {
            var d = source.AddMilliseconds(-source.Millisecond);
            d = d.AddSeconds(-source.Second);
            d = d.AddMinutes(-source.Minute);
            if (d != source)
            {
                d = d.AddHours(1);
            }
            return d;
        }
    }
}