using Electricity.Application.Common.Extensions;
using Electricity.Application.Common.Extensions.ITimeSeries;
using Electricity.Application.Common.Models.TimeSeries;
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

        public float[] GetPeakDemandInMonths(PowerQuantity quantity)
        {
            var i = GetIndexOfQuantity(quantity);
            var series = new VariableIntervalTimeSeries<float[]>(_rows);
            return series.ChunkByMonth().Select(ch =>
            {
                var avgValues = ch.AggregateQuarterHours((rows) =>
                {
                    var sum = rows.Aggregate<float[], float>(0, (acc, row) => acc + row[i]);
                    return sum / rows.Count();
                });
                return avgValues.Values().Max();
            }).ToArray();
        }
    }
}