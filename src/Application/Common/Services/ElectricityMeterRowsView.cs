using DataSource;
using Electricity.Application.Common.Extensions;
using Electricity.Application.Common.Extensions.ITimeSeries;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public class ElectricityMeterRowsView : RowsView<ElectricityMeterQuantity>
    {
        public ElectricityMeterRowsView(
            ElectricityMeterQuantity[] quantities,
            IEnumerable<Tuple<DateTime, float[]>> rows
            ) : base(quantities, rows)
        {
        }

        public float GetDifference(ElectricityMeterQuantity quantity)
        {
            var i = GetIndexOfQuantity(quantity);
            var firstRow = _rows.First();
            var lastRow = _rows.Last();

            return lastRow.Item2[i] - firstRow.Item2[i];
        }

        public VariableIntervalTimeSeries<float> GetDifferenceInMonths(ElectricityMeterQuantity quantity)
        {
            var i = GetIndexOfQuantity(quantity);
            var series = new VariableIntervalTimeSeries<float[]>(_rows.ToArray());
            var prevValue = series.Entries().First().Item2[i];
            var result = series.ChunkByMonth().Select((ch) =>
            {
                var currValue = ch.Entries().Last().Item2[i];
                var diff = currValue - prevValue;
                prevValue = currValue;
                var startTime = ch.StartTime.FloorMonth();
                return Tuple.Create(startTime, diff);
            });
            return new VariableIntervalTimeSeries<float>(result.ToArray());
        }

        public VariableIntervalTimeSeries<float> GetDifferenceInQuarterHours(ElectricityMeterQuantity quantity)
        {
            var i = GetIndexOfQuantity(quantity);
            var series = new VariableIntervalTimeSeries<float[]>(_rows.ToArray());
            var prevValue = series.Entries().First().Item2[i];
            var result = series.ChunkByQuarterHour().Select((ch) =>
            {
                var currValue = ch.Entries().Last().Item2[i];
                var diff = currValue - prevValue;
                prevValue = currValue;
                var startTime = ch.StartTime.FloorQuarterHour();
                return Tuple.Create(startTime, diff);
            });
            return new VariableIntervalTimeSeries<float>(result.ToArray());
        }

        public VariableIntervalTimeSeries<float> GetTimeSeries(ElectricityMeterQuantity quantity)
        {
            var i = GetIndexOfQuantity(quantity);
            var entries = _rows.Select(row => Tuple.Create(row.Item1, row.Item2[i]));
            return new VariableIntervalTimeSeries<float>(entries.ToArray());
        }
    }
}