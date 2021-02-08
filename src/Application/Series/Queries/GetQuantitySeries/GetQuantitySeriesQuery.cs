using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Queries;
using MediatR;

namespace Electricity.Application.Series.Queries.GetQuantitySeries
{
    public class GetQuantitySeriesQuery : IRequest<QuantitySeriesDto>
    {
        public Guid GroupId { get; set; }

        public byte Arch { get; set; }

        public Quantity Quantity { get; set; }

        public Interval Range { get; set; }

        public AggregationMethod AggregationMethod { get; set; }

        public TimeSpan AggregationInterval { get; set; }
    }

    public class GetQuantitySeriesQueryHandler : IRequestHandler<GetQuantitySeriesQuery, QuantitySeriesDto>
    {
        private readonly ITableCollection _tables;
        private readonly IMapper _mapper;

        public GetQuantitySeriesQueryHandler(ITableCollection tables, IMapper mapper)
        {
            _tables = tables;
            _mapper = mapper;
        }

        public Task<QuantitySeriesDto> Handle(GetQuantitySeriesQuery request, CancellationToken cancellationToken)
        {
            var table = _tables.GetTable(request.GroupId, request.Arch);

            var rows = table.GetRows(new GetRowsQuery
            {
                Quantities = new Quantity[] { request.Quantity },
                Aggregation = (uint)Math.Floor(request.AggregationInterval.TotalMilliseconds),
                Range = request.Range
            });

            var dto = new QuantitySeriesDto
            {
                Entries = rows.Select(r => Tuple.Create(r.Item1, r.Item2[0])).ToList()
            };

            return Task.FromResult(dto);
        }
    }
}