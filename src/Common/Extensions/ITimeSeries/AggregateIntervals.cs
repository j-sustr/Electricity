using Common.Extension;
using Common.Temporal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Series
{
    public static partial class TimeSeriesExtensions
    {
        public static FixedIntervalTimeSeries<V> AggregateIntervals<V, TAggregate>(this ITimeSeries<V> series, Func<IEnumerable<V>, TAggregate> aggregator, Func<DateTime, int> indexResolver)
        {
            var entries = series.GetEntries();
            foreach (var chunk in entries.ChunkByIndex((entry, i) => indexResolver(entry.Item1)))
            {
                TAggregate value;
                value = aggregator(chunk?.Select(entry => entry.Item2));
            }

            return new FixedIntervalTimeSeries<V>(new V[] { }, DateTime.Now, TimeSpan.FromMinutes(1));
        }

        // static  FindExtremeInterval
    }
}