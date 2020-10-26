using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Quantities.Queries.GetQuantities
{
    internal class GetQuantitiesQuery : IRequest<QuantitiesDto>
    {
        public GetQuantitiesQuery()
        {
        }
    }
}