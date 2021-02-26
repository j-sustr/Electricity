﻿using Electricity.Application.Common.Models.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Extensions.ITimeSeries
{
    public static partial class TimeSeriesExtensions
    {
        public static IEnumerable<VariableIntervalTimeSeries<TValue>> ChunkByMonth<TValue, TAggregate>(this ITimeSeries<TValue> source)
        {
            var start = source.StartTime.FloorMonth();
            var chunks = source.Entries().ChunkByIndex((item, _) =>
            {
                return start.CountMonthsTo(item.Item1);
            });

            return chunks.Select(ch =>
            {
                if (ch == null)
                {
                    return null;
                }
                return new VariableIntervalTimeSeries<TValue>(ch);
            });
        }
    }
}