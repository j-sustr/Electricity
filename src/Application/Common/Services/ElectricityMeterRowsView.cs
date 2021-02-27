using DataSource;
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

        public float[] GetDifferenceInMonths(ElectricityMeterQuantity quantity)
        {
            var i = GetIndexOfQuantity(quantity);
            var series = new VariableIntervalTimeSeries<float[]>(_rows);
            var prevValue = series.Entries().First().Item2[i];
            return series.ChunkByMonth().Select((pair) =>
            {
                var currValue = series.Entries().Last().Item2[i];
                var diff = currValue - prevValue;
                prevValue = currValue;
                return diff;
            }).ToArray();
        }
    }
}