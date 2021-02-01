using System;
using System.Collections.Generic;

namespace Electricity.Application.Series.Queries.GetQuantitySeries
{
    public class QuantitySeriesDto
    {
        public IList<Tuple<DateTime, float>> Entries { get; set; }
    }
}