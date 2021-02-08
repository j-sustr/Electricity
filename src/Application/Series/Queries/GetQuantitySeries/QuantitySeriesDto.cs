using Electricity.Application.Common.Models;
using System;
using System.Collections.Generic;

namespace Electricity.Application.Series.Queries.GetQuantitySeries
{
    public class QuantitySeriesDto
    {
        public Interval Interval { get; set; }
        public IList<Tuple<DateTime, float>> Entries { get; set; }
    }
}