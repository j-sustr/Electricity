using Electricity.Application.Common.Extensions;
using Electricity.Application.Common.Extensions.ITimeSeries;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.TimeSeries;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public class PowerRowsView : RowsView<PowerQuantity>
    {
        public PowerRowsView(
            PowerQuantity[] quantities,
            IEnumerable<Tuple<DateTime, float[]>> rows
            ) : base(quantities, rows)
        {
        }

        public IEnumerable<PeakDemandItem> GetPeakDemandInMonths(PowerQuantity quantity)
        {
            var i = GetIndexOfQuantity(quantity);
            var series = new VariableIntervalTimeSeries<float[]>(_rows.ToArray());
            return series.ChunkByMonth().Select(ch =>
            {
                var avgValues = ch.AggregateQuarterHours((rows) =>
                {
                    var sum = rows.Aggregate<float[], float>(0, (acc, row) => acc + row[i]);
                    return sum / rows.Count();
                });
                var maxEntry = avgValues.Entries().MaxBy((ent) => ent.Item2).First();
                return new PeakDemandItem
                {
                    IntervalStart = ch.StartTime.FloorMonth(),
                    Start = maxEntry.Item1,
                    Value = maxEntry.Item2
                };
            });
        }

        public FixedIntervalTimeSeries<float> GetDemandSeries(PowerQuantity quantity)
        {
            var i = GetIndexOfQuantity(quantity);
            var series = new VariableIntervalTimeSeries<float[]>(_rows.ToArray());
            var avgSeries = series.AggregateQuarterHours((rows) =>
            {
                var sum = rows.Aggregate<float[], float>(0, (acc, row) => acc + row[i]);
                return sum / rows.Count();
            });
            return avgSeries;
        }
    }
}