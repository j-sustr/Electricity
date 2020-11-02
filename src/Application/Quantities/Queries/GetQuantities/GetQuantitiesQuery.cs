using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataSource;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Interfaces;
using MediatR;
using System;
using System.Linq;
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
            Quantity[] quants = null;

            quants = _service.GetQuantities(request.GroupId, request.Arch, null);

            if (quants == null)
            {
                throw new NotFoundException(nameof(Group), request.GroupId);
            }

            var quantsDto = _mapper.Map<Quantity[], QuantityDto[]>(quants);

            return await Task.FromResult(new QuantitiesDto
            {
                List = quantsDto.ToList()
            });
        }
    }
}