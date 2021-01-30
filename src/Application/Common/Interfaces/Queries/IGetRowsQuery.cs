using System;
using DataSource;

namespace Electricity.Application.Common.Interfaces.Queries
{
    public interface IGetRowsQuery
    {
        Tuple<DateTime, DateTime> Range { get; }
        Quantity[] Quantities { get; }
        uint Aggregation { get; }
        EEnergyAggType EnergyAggType { get; }
    }
}