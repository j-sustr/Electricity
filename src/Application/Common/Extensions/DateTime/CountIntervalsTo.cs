using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Extensions
{
    public static partial class DateTimeExtensions
    {
        public static int CountYearsTo(this DateTime source, DateTime target)
        {
            return target.Year - source.Year;
        }

        public static int CountMonthsTo(this DateTime source, DateTime target)
        {
            int y = source.CountYearsTo(target);
            return target.Month - source.Month + y * 12;
        }

        public static int CountDaysTo(this DateTime source, DateTime target)
        {
            var ts = target - source;
            return (int)Math.Floor(ts.TotalDays);
        }

        public static int CountQuarterHoursTo(this DateTime source, DateTime target)
        {
            int minutes = source.CountMinutesTo(target);
            return minutes / 15;
        }

        public static int CountMinutesTo(this DateTime source, DateTime target)
        {
            var ts = target - source;
            return (int)Math.Floor(ts.TotalMinutes);
        }
    }
}