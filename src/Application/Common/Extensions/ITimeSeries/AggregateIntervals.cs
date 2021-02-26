using Electricity.Application.Common.Models.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Extensions
{
    public static partial class TimeSeriesExtensions
    {
        public static FixedIntervalTimeSeries<TValue> AggregateIntervals<TValue, TAggregate>(this ITimeSeries<TValue> series, Func<IEnumerable<TValue>, TAggregate> aggregator, Func<DateTime, int> indexResolver)
        {
            var entries = series.Entries();
            foreach (var chunk in entries.ChunkByIndex((entry, i) => indexResolver(entry.Item1)))
            {
                TAggregate value;
                value = aggregator(chunk?.Select(entry => entry.Item2));
            }

            return new FixedIntervalTimeSeries<TValue>(new TValue[] { }, DateTime.Now, TimeSpan.FromMinutes(1));
        }

        // static  FindExtremeInterval
    }
}