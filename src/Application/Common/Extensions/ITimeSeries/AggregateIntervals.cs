using Electricity.Application.Common.Models.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Extensions
{
    public static partial class TimeSeriesExtensions
    {
        public static FixedIntervalTimeSeries<TAggregate> AggregateQuarterHours<TValue, TAggregate>(this ITimeSeries<TValue> source, Func<IEnumerable<TValue>, TAggregate> aggregator)
        {
            var start = source.StartTime.FloorQuarterHour();
            var values = source.AggregateIntervals(aggregator, (d) =>
            {
                return start.CountQuarterHoursTo(d);
            });

            return new FixedIntervalTimeSeries<TAggregate>(values, start, TimeSpan.FromMinutes(15));
        }

        public static TAggregate[] AggregateIntervals<TValue, TAggregate>(this ITimeSeries<TValue> source, Func<IEnumerable<TValue>, TAggregate> aggregator, Func<DateTime, int> indexResolver)
        {
            var entries = source.Entries();
            var result = new List<TAggregate>();
            foreach (var chunk in entries.ChunkByIndex((entry, i) => indexResolver(entry.Item1)))
            {
                var value = aggregator(chunk?.Select(entry => entry.Item2));
                result.Add(value);
            }

            return result.ToArray();
        }
    }
}