using System;
using Ardalis.GuardClauses;
using KMB.DataSource;

namespace Electricity.Application.Common.Extensions
{
    public static class DateRangeExtensions
    {
        public static DateRange FromTuple(Tuple<DateTime, DateTime> range)
        {
            Guard.Against.Null(range, nameof(range));

            return new DateRange(range.Item1, range.Item2);
        }
    }
}