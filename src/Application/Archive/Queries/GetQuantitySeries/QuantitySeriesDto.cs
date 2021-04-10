using Electricity.Application.Common.Models;
using System;
using System.Collections.Generic;

namespace Electricity.Application.Archive.Queries.GetQuantitySeries
{
    public class QuantitySeriesDto
    {
        public IList<Tuple<DateTime, float>> Entries { get; set; }
    }
}