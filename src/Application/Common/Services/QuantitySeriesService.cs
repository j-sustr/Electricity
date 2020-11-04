using Common.Series;
using DataSource;
using Electricity.Application.Common.Extensions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.SystemMonitoring;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public class QuantitySeriesService
    {
        private readonly IRowCollectionReader _reader;

        private readonly MetricsStore _metricsStore;

        public QuantitySeriesService(IRowCollectionReader reader, MetricsStore metricsStore)
        {
            _reader = reader;
            _metricsStore = metricsStore;
        }

        unsafe public ITimeSeries<float> GetSeries(IGetQuantitySeriesQuery query)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            var quants = new Quantity[] { query.Quantity };
            PropValueFloatBase[] floatsQ = null;

            var entries = new List<Tuple<DateTime, float>>();
            float? val;
            var dateRange = query.Range != null ? DateRangeExtensions.FromTuple(query.Range) : null;
            using (var rc = _reader.GetRows(query.GroupId, query.Arch, dateRange, quants, 3600000, EEnergyAggType.Profile))
            {
                fixed (byte* p = rc.Buffer)
                {
                    rc.SetPointer(p);
                    foreach (RowInfo row in rc)
                    {
                        sw.Start();
                        val = quants[0].Value.GetValue() as float?;
                        sw.Stop();
                        entries.Add(Tuple.Create(row.TimeLocal, val ?? float.NaN));
                    }
                }
            }

            _metricsStore.AddRecord("QuantitySeriesService.GetSeries", sw.Elapsed, query);

            return new VariableIntervalTimeSeries<float>(entries);
        }
    }
}