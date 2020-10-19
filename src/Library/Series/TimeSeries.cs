using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Series
{
    internal static class TimeSeries<V>
    {
        private class AggregationParams
        {
        }

        private static FixedIntervalTimeSeries<V> Aggregate(ITimeSeries<V> series, AggregationParams aggregationParams)
        {
            return new FixedIntervalTimeSeries<V>(new V[] { }, DateTime.Now, TimeSpan.FromMinutes(1));
        }

        // static  FindExtremeInterval
    }
}