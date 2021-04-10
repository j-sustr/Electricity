using AutoMapper;
using AutoMapper.QueryableExtensions;
using KMB.DataSource;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.Archive.Queries.GetQuantities
{
    public class GetQuantitiesQuery : IRequest<QuantitiesDto>
    {
        public Guid GroupId { get; set; }
        public byte Arch { get; set; }

        // public Tuple<DateTime, DateTime> Range { get; set; }
    }

    public class GetQuantitiesQueryHandler : IRequestHandler<GetQuantitiesQuery, QuantitiesDto>
    {
        private readonly IArchiveRepository _repo;
        private readonly IMapper _mapper;

        public GetQuantitiesQueryHandler(IArchiveRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<QuantitiesDto> Handle(GetQuantitiesQuery request, CancellationToken cancellationToken)
        {
            var arch = _repo.GetArchive(request.GroupId, request.Arch);
            if (arch == null)
            {
                throw new NotFoundException($"Archive ({request.GroupId}, {request.Arch})");
            }

            Quantity[] quants = arch.GetQuantities(null);
            if (quants == null)
            {
                return await Task.FromResult<QuantitiesDto>(null);
            }

            var quantsDto = _mapper.Map<Quantity[], QuantityDto[]>(quants);

            return await Task.FromResult(new QuantitiesDto
            {
                List = quantsDto.ToList()
            });
        }
    }
}