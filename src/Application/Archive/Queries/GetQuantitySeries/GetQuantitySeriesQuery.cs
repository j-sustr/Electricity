using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KMB.DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Queries;
using MediatR;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Common.Exceptions;

namespace Electricity.Application.Archive.Queries.GetQuantitySeries
{
    public class GetQuantitySeriesQuery : IRequest<QuantitySeriesDto>
    {
        public Guid GroupId { get; set; }

        public byte Arch { get; set; }

        public IntervalDto Range { get; set; }

        public string PropName { get; set; }

        public string? Unit { get; set; }

        public int? Aggregation { get; set; }

        public EEnergyAggType? EnergyAggType { get; set; }
    }

    public class GetQuantitySeriesQueryHandler : IRequestHandler<GetQuantitySeriesQuery, QuantitySeriesDto>
    {
        private readonly IArchiveRepository _archiveRepo;
        private readonly IMapper _mapper;

        public GetQuantitySeriesQueryHandler(IArchiveRepository tables, IMapper mapper)
        {
            _archiveRepo = tables;
            _mapper = mapper;
        }

        public Task<QuantitySeriesDto> Handle(GetQuantitySeriesQuery request, CancellationToken cancellationToken)
        {
            var range = _mapper.Map<Interval>(request.Range);

            var archive = _archiveRepo.GetArchive(request.GroupId, request.Arch);
            if (archive == null)
                throw new NotFoundException("archive not found");

            var quantity = new Quantity(request.PropName, request.Unit ?? null);

            var rows = archive.GetRows(new GetArchiveRowsQuery
            {
                Range = range,
                Quantities = new Quantity[] { quantity },
                Aggregation = (uint)(request.Aggregation ?? 0),
                EnergyAggType = request.EnergyAggType ?? EEnergyAggType.Cumulative
            });

            var dto = new QuantitySeriesDto
            {
                Entries = rows.Select(r =>
                {
                    return new object[2] { r.Item1, r.Item2[0] };

                }).ToArray()
            };

            return Task.FromResult(dto);
        }
}
}