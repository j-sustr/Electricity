using System.Collections.Generic;

namespace Electricity.Application.Quantities.Queries.GetQuantities
{
    public class QuantitiesDto
    {
        public IList<QuantityDto> List { get; set; } = new List<QuantityDto>();
    }
}