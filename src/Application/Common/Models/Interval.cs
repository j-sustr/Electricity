using System;
using DataSource;
using Electricity.Application.Common.Utils;

namespace Electricity.Application.Common.Models
{
    public class Interval : IEquatable<Interval>
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Interval(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public Interval GetOverlap(Interval other)
        {
            var max = DateTimeUtil.Earliest(End, other.End);
            var min = DateTimeUtil.Latest(Start, other.Start);

            if (min >= max)
            {
                return null;
            }

            return new Interval(min, max);
        }

        public static Interval FromDateRange(DateRange dateRange)
        {
            return new Interval(dateRange.DateMin, dateRange.DateMax);
        }

        public bool Equals(Interval other)
        {
            return Start == other.Start && End == other.End;
        }
    }
}