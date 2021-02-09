using System;
using DataSource;
using Electricity.Application.Common.Utils;
using Newtonsoft.Json;

namespace Electricity.Application.Common.Models
{
    public class Interval : IEquatable<Interval>
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

        [JsonIgnore]
        public bool IsFinite
        {
            get
            {
                return Start != null && End != null;
            }
        }

        [JsonIgnore]
        public bool IsHalfBounded
        {
            get
            {
                return !IsFinite && Start != End;
            }
        }

        public Interval(DateTime? start, DateTime? end)
        {
            if (start > end)
            {
                throw new ArgumentException("start is after end");
            }

            Start = start;
            End = end;
        }

        public Interval GetOverlap(Interval other)
        {
            var max = DateTimeUtil.Earliest(End ?? DateTime.MaxValue, other.End ?? DateTime.MaxValue);
            var min = DateTimeUtil.Latest(Start ?? DateTime.MinValue, other.Start ?? DateTime.MinValue);

            if (min >= max)
            {
                return null;
            }

            return new Interval(min, max);
        }

        public bool Equals(Interval other)
        {
            return Start == other.Start && End == other.End;
        }

        public DateRange ToDateRange()
        {
            if (!IsFinite)
            {
                return null;
            }

            return new DateRange((DateTime)Start, (DateTime)End);
        }

        public static Interval Unbounded
        {
            get
            {
                return new Interval(null, null);
            }
        }

        public static Interval FromDateRange(DateRange dateRange)
        {
            return new Interval(dateRange.DateMin, dateRange.DateMax);
        }
    }
}