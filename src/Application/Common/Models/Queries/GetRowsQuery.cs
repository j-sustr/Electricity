using System;
using DataSource;
using Electricity.Application.Common.Interfaces.Queries;

namespace Electricity.Application.Common.Models.Queries
{
    public class GetRowsQuery : IGetRowsQuery
    {
        public Tuple<DateTime, DateTime> Range { get; set; }

        public Quantity[] Quantities { get; set; }

        public uint Aggregation { get; set; }

        public EEnergyAggType EnergyAggType { get; set; } = EEnergyAggType.Cumulative;
    }
}