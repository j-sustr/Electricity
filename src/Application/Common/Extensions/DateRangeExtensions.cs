using System;
using DataSource;

namespace Electricity.Application.Common.Extensions
{
    public static class DateRangeExtensions
    {
        public static DateRange FromTuple(Tuple<DateTime, DateTime> range)
        {
            return new DateRange(range.Item1, range.Item2);
        }
    }
}