using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Extensions;
using Electricity.Application.Common.Models.TimeSeries;
using Electricity.Application.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.PeakDemand
{
    static class PeakDemand
    {
        public static MainQuantity[] GetQuantities(Phase[] phases)
        {
            var quanities = new List<MainQuantity>();

            foreach (var p in phases)
            {
                quanities.Add(new MainQuantity
                {
                    Type = MainQuantityType.PAvg,
                    Phase = p
                });
            }

            return quanities.ToArray();
        }

        public static FixedIntervalTimeSeries<float> AggregateSeries(ITimeSeries<float> series, DemandAggregation aggregation)
        {
            DateTime startTime;
            TimeSpan offsetDuration;
            switch (aggregation)
            {
                case DemandAggregation.OneHour:
                    startTime = series.StartTime.FloorHour();
                    offsetDuration = series.StartTime.CeilHour() - series.StartTime;
                    break;
                case DemandAggregation.SixHours:
                    offsetDuration = series.StartTime.CeilDay() - series.StartTime;
                    offsetDuration -= TimeSpan.FromHours(((int)offsetDuration.TotalHours / 6) * 6);
                    startTime = series.StartTime + offsetDuration - TimeSpan.FromHours(6);
                    break;
                case DemandAggregation.TwelveHours:
                    offsetDuration = series.StartTime.CeilDay() - series.StartTime;
                    offsetDuration -= TimeSpan.FromHours(((int)offsetDuration.TotalHours / 12) * 12);
                    startTime = series.StartTime + offsetDuration - TimeSpan.FromHours(12);
                    break;
                case DemandAggregation.OneDay:
                    startTime = series.StartTime.FloorDay();
                    offsetDuration = series.StartTime.CeilDay() - series.StartTime;
                    break;
                case DemandAggregation.OneWeek:
                    startTime = series.StartTime.FloorWeek();
                    offsetDuration = series.StartTime.CeilWeek() - series.StartTime;
                    break;
                default:
                    throw new ArgumentException("invalid aggregation", nameof(aggregation));
            }

            int chunkSize = (int)aggregation;
            int offset = (int)(offsetDuration / TimeSpan.FromMinutes(15));

            var aggregatedValues = series.Values()
                .Chunk(chunkSize, offset)
                .Select(chunk =>
                {
                    int indexOfMax = chunk.Select(value => MathF.Abs(value)).IndexOfMax();
                    return chunk[indexOfMax];
                })
                .ToArray();

            var interval = chunkSize * TimeSpan.FromMinutes(15);
            return new FixedIntervalTimeSeries<float>(aggregatedValues, startTime, interval);
        }
    }
}
