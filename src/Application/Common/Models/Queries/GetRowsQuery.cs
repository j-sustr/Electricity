using System;
using KMB.DataSource;

namespace Electricity.Application.Common.Models.Queries
{
    public class GetRowsQuery
    {
        public Interval Interval { get; set; } = Interval.Unbounded;

        public Quantity[] Quantities { get; set; }

        public uint Aggregation { get; set; } = 0;

        public EEnergyAggType EnergyAggType { get; set; } = EEnergyAggType.Cumulative;
    }
}