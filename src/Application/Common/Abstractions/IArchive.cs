using System;
using System.Collections.Generic;
using Electricity.Application.Common.Models;
using KMB.DataSource;

namespace Electricity.Application.Common.Interfaces
{
    public class GetArchiveRowsQuery
    {
        public Interval Range { get; set; } = Interval.Unbounded;

        public Quantity[] Quantities { get; set; }

        public uint Aggregation { get; set; } = 0;

        public EEnergyAggType EnergyAggType { get; set; } = EEnergyAggType.Cumulative;
    }

    public interface IArchive
    {
        Quantity[] GetQuantities(DateRange range);

        IEnumerable<Tuple<DateTime, float[]>> GetRows(GetArchiveRowsQuery query);
    }
}