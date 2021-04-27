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
    public class MainRowsView : RowsView<MainQuantity>
    {
        public MainRowsView(
            MainQuantity[] quantities,
            IEnumerable<Tuple<DateTime, float[]>> rows
            ) : base(quantities, rows)
        {
        }

        public IEnumerable<PeakDemandItem> GetPeakDemands(MainQuantity quantity)
        {
            var i = GetIndexOfQuantity(quantity);
            var series = new VariableIntervalTimeSeries<float[]>(_rows.ToArray());
            return series.Entries()
                .MaxBy((ent) => ent.Item2[i])
                .Select(ent =>
            {
                return new PeakDemandItem
                {
                    Start = ent.Item1,
                    Value = ent.Item2[i]
                };
            });
        }

        public IEnumerable<PeakDemandInMonth> GetPeakDemandInMonths(MainQuantity quantity)
        {
            var i = GetIndexOfQuantity(quantity);
            var series = new VariableIntervalTimeSeries<float[]>(_rows.ToArray());
            return series.ChunkByMonth().Select(ch =>
            {
                var maxEntry = ch.Entries().MaxBy((ent) => MathF.Abs(ent.Item2[i])).First();
                return new PeakDemandInMonth
                {
                    MonthStart = ch.StartTime.FloorMonth(),
                    Start = maxEntry.Item1,
                    Value = maxEntry.Item2[i]
                };
            });
        }

        // --- DEAD CODE ---

        public FixedIntervalTimeSeries<float> GetDemandSeries(MainQuantity quantity)
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