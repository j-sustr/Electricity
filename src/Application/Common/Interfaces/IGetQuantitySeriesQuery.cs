using DataSource;
using Electricity.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Interfaces
{
    public interface IGetQuantitySeriesQuery
    {
        Guid GroupId { get; }
        byte Arch { get; }

        Quantity Quantity { get; }
        Tuple<DateTime, DateTime> Range { get; }
        AggregationMethod AggregationMethod { get; }
        TimeSpan AggregationInterval { get; }
    }
}