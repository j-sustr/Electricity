using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataSource;
using Electricity.Application.Common.Extensions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.Quantities.Queries.GetQuantities
{
    public class GetQuantitiesQuery : IRequest<QuantitiesDto>
    {
        public Guid GroupId { get; set; }
        public byte Arch { get; set; }

        // public Tuple<DateTime, DateTime> Range { get; set; }
    }

    public class GetQuantitiesQueryHandler : IRequestHandler<GetQuantitiesQuery, QuantitiesDto>
    {
        private readonly IQuantityService _service;
        private readonly IMapper _mapper;
        public GetQuantitiesQueryHandler(IQuantityService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<QuantitiesDto> Handle(GetQuantitiesQuery request, CancellationToken cancellationToken)
        {
            // var dateRange = DateRangeExtensions.FromTuple(request.Range);

            var quants = _service.GetQuantities(request.GroupId, request.Arch, null);

            return await Task.FromResult(new QuantitiesDto
            {
                List = Queryable.AsQueryable(quants)
                                .ProjectTo<QuantityDto>(_mapper.ConfigurationProvider).ToList()
            });
        }
    }
}