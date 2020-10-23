using System;
using DataSource;

namespace Electricity.Application.Common.Interfaces
{
    public interface IRowCollectionReader
    {
        RowCollection GetRows(Guid groupId, byte arch, DateRange range, Quantity[] quantities, uint aggregation, EEnergyAggType energyAggType = EEnergyAggType.Cumulative);
    }
}