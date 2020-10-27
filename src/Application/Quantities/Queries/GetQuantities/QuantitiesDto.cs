using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Quantities.Queries.GetQuantities
{
    public class QuantitiesDto
    {
        public IList<QuantityDto> List { get; set; } = new List<QuantityDto>();
    }
}