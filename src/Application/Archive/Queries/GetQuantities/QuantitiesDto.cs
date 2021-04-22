using System.Collections.Generic;

namespace Electricity.Application.Archive.Queries.GetQuantities
{
    public class QuantitiesDto
    {
        public IList<QuantityDto> List { get; set; } = new List<QuantityDto>();
    }
}