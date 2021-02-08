using System;
using DataSource;

namespace Electricity.Application.Common.Models.Queries
{
    public class GetRowsQuery
    {
        public Interval? Range { get; set; } = null;

        public Quantity[] Quantities { get; set; }

        public uint Aggregation { get; set; } = 0;

        public EEnergyAggType EnergyAggType { get; set; } = EEnergyAggType.Cumulative;
    }
}