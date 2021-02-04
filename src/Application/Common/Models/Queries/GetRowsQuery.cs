using System;
using DataSource;

namespace Electricity.Application.Common.Models.Queries
{
    public class GetRowsQuery
    {
        public Tuple<DateTime, DateTime> Range { get; set; }

        public Quantity[] Quantities { get; set; }

        public uint Aggregation { get; set; } = 0;

        public EEnergyAggType EnergyAggType { get; set; } = EEnergyAggType.Cumulative;
    }
}