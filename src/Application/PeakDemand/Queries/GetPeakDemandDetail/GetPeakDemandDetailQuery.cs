using AutoMapper;
using Electricity.Application.Common.Constants;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Extensions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Common.Models.TimeSeries;
using Electricity.Application.Common.Services;
using Electricity.Application.PeakDemand.Queries.GetPeakDemandOverview;
using KMB.DataSource;
using MediatR;
using MoreLinq;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.PeakDemand.Queries.GetPeakDemandDetail
{
    using static PeakDemand;

    public class GetPeakDemandDetailQuery : IRequest<PeakDemandDetailDto>
    {
        public string GroupId { get; set; }

        public IntervalDto Interval1 { get; set; }
        public IntervalDto? Interval2 { get; set; }
        public Phases Phases { get; set; }

        public DemandAggregation Aggregation { get; set; }

    }

    public class GetPeakDemandDetailQueryHandler : IRequestHandler<GetPeakDemandDetailQuery, PeakDemandDetailDto>
    {
        private readonly ArchiveRepositoryService _archiveRepoService;
        private readonly IGroupRepository _groupService;
        private readonly IMapper _mapper;

        public GetPeakDemandDetailQueryHandler(
            ArchiveRepositoryService archiveRepoService,
            IGroupRepository groupService,
            IMapper mapper)
        {
            _archiveRepoService = archiveRepoService;
            _groupService = groupService;
            _mapper = mapper;
        }

        public Task<PeakDemandDetailDto> Handle(GetPeakDemandDetailQuery request, CancellationToken cancellationToken)
        {
            var groupInfo = _groupService.GetGroupInfo(request.GroupId);
            if (groupInfo == null)
                throw new NotFoundException("group not found");

            var interval1 = _mapper.Map<Interval>(request.Interval1);
            var interval2 = _mapper.Map<Interval>(request.Interval2);
            var phases = request.Phases;

            var demandSeries1 = GetDemandSeriesForInterval(groupInfo, interval1, phases, request.Aggregation, nameof(request.Interval1));
            var demandSeries2 = GetDemandSeriesForInterval(groupInfo, interval2, phases, request.Aggregation, nameof(request.Interval2));

            return Task.FromResult(new PeakDemandDetailDto
            {
                DemandSeries1 = demandSeries1,
                DemandSeries2 = demandSeries2
            });
        }

        private DemandSeriesDto GetDemandSeriesForInterval(
            GroupInfo group, Interval interval, Phases phases, DemandAggregation aggregation, string intervalName)
        {
            if (interval == null) return null;

            var quantities = GetQuantities(phases.ToArray());

            var mainView = _archiveRepoService.GetMainRowsView(new GetMainRowsViewQuery
            {
                GroupId = group.ID,
                Range = interval,
                Quantities = quantities,
                Aggregation = ApplicationConstants.MAIN_AGGREGATION
            });
            var resultInterval = mainView.GetInterval();

            var demands = new Dictionary<Phase, float[]>();
            demands.Add(Phase.Main, null);
            demands.Add(Phase.L1, null);
            demands.Add(Phase.L2, null);
            demands.Add(Phase.L3, null);

            foreach (var phase in phases.ToArray())
            {
                var entries = mainView.GetSeries(new MainQuantity
                {
                    Type = MainQuantityType.PAvg,
                    Phase = phase
                });
                ITimeSeries<float> series = new VariableIntervalTimeSeries<float>(entries.ToArray());
                if (aggregation != DemandAggregation.None)
                {
                    series = AggregateSeries(series, aggregation);
                }
                demands[phase] = series.Values().ToArray();
            }

            return new DemandSeriesDto
            {
                TimeRange = _mapper.Map<IntervalDto>(resultInterval),
                TimeStep = ApplicationConstants.MAIN_AGGREGATION * (int)aggregation,
                ValuesMain = demands[Phase.Main],
                ValuesL1 = demands[Phase.L1],
                ValuesL2 = demands[Phase.L2],
                ValuesL3 = demands[Phase.L3]
            };
        }
    }
}
