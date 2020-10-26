using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Quantities.Queries.GetQuantities
{
    internal class QuantitiesDto
    {
        private IList<QuantityDto> List { get; set; } = new List<QuantityDto>();
    }
}