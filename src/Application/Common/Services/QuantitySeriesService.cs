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
            int cnt = 0;
            int i;
            float val, sum;
            var dateRange = DateRangeExtensions.FromTuple(query.Range);
            using (var rc = _reader.GetRows(query.GroupId, query.Arch, dateRange, quants, 0))
            {
                UniArchiveDefinition uad = null;
                fixed (byte* p = rc.Buffer)
                {
                    rc.SetPointer(p);
                    foreach (RowInfo row in rc)
                    {
                        if (uad != row.uad)
                        {
                            // if (uad == null) Len = row.uad.tbd.Len;//tohle mam jen na testovani
                            uad = row.uad;//funguje momentalne jen bez agregace, s agregaci je null
                            floatsQ = quants.Select(a => a.Value as PropValueFloatBase).Where(a => a != null).ToArray();//tady se vyberou jen float hodnoty a vytvori se pole PropValueFloat
                            cnt = floatsQ.Length;
                        }
                        sum = 0;
                        sw.Start();
                        for (i = 0; i < cnt; i++)
                        {
                            //val = (float)quants[i].Value.GetValue();
                            val = floatsQ[i].GetValueDirect();//nacitam floaty jednotlivych Quantity
                            if (!ByteArray.IsNaNInfinityOrMaxMin(val)) sum += val;
                        }
                        sw.Stop();
                        entries.Add(Tuple.Create(row.TimeLocal, sum));
                    }
                }
            }

            _metricsStore.AddRecord("QuantitySeriesService.GetSeries", sw.Elapsed, query);

            return new VariableIntervalTimeSeries<float>(entries);
        }
    }
}