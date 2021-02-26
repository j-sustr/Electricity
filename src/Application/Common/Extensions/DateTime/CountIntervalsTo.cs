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
    }
}