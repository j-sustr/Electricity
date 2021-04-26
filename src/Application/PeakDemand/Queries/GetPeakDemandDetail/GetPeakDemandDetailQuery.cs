using AutoMapper;
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
    public class GetPeakDemandDetailQuery : IRequest<PeakDemandDetailDto>
    {
        public string GroupId { get; set; }

        public IntervalDto Interval1 { get; set; }

        public IntervalDto? Interval2 { get; set; }

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
            var interval1 = _mapper.Map<Interval>(request.Interval1);
            var interval2 = _mapper.Map<Interval>(request.Interval2);

            var groupInfo = _groupService.GetGroupInfo(request.GroupId);
            if (groupInfo == null)
                throw new NotFoundException("group not found");

            var demandSeries1 = GetDemandSeriesForInterval(groupInfo, interval1, nameof(request.Interval1), request.Aggregation);
            var demandSeries2 = GetDemandSeriesForInterval(groupInfo, interval2, nameof(request.Interval2), request.Aggregation);

            return Task.FromResult(new PeakDemandDetailDto
            {
                DemandSeries1 = demandSeries1,
                DemandSeries2 = demandSeries2
            });
        }

        private DemandSeriesDto GetDemandSeriesForInterval(GroupInfo group, Interval interval, string intervalName, DemandAggregation aggregation)
        {
            if (interval == null) return null;

            var powQuantities = new MainQuantity[] {
                    new MainQuantity
                    {
                        Type = MainQuantityType.PAvg,
                        Phase = Phase.Main
                    }
                };

            var powView = _archiveRepoService.GetMainRowsView(new GetMainRowsViewQuery
            {
                GroupId = group.ID,
                Range = interval,
                Quantities = powQuantities
            });
            var resultInterval = powView.GetInterval();

            var seriesMain = powView.GetDemandSeries(new MainQuantity
            {
                Type = MainQuantityType.PAvg,
                Phase = Phase.Main
            });
            if (aggregation != DemandAggregation.None)
            {
                seriesMain = AggregateSeries(seriesMain, aggregation);
            }

            var valuesMain = seriesMain.Values().ToArray();

            return new DemandSeriesDto
            {
                TimeRange = _mapper.Map<IntervalDto>(resultInterval),
                TimeStep = (int)TimeSpan.FromMinutes(15 * (int)aggregation).TotalMilliseconds, // get timestep from request.Aggregation
                ValuesMain = valuesMain
            };
        }

        public FixedIntervalTimeSeries<float> AggregateSeries(FixedIntervalTimeSeries<float> series, DemandAggregation aggregation)
        {
            DateTime startTime;
            TimeSpan offsetDuration;
            switch (aggregation)
            {
                case DemandAggregation.OneHour:
                    startTime = series.StartTime.FloorHour();
                    offsetDuration = series.StartTime.CeilHour() - series.StartTime;
                    break;
                case DemandAggregation.SixHours:
                    offsetDuration = series.StartTime.CeilDay() - series.StartTime;
                    offsetDuration -= TimeSpan.FromHours(((int)offsetDuration.TotalHours / 6) * 6);
                    startTime = series.StartTime + offsetDuration - TimeSpan.FromHours(6);
                    break;
                case DemandAggregation.TwelveHours:
                    offsetDuration = series.StartTime.CeilDay() - series.StartTime;
                    offsetDuration -= TimeSpan.FromHours(((int)offsetDuration.TotalHours / 12) * 12);
                    startTime = series.StartTime + offsetDuration - TimeSpan.FromHours(12);
                    break;
                case DemandAggregation.OneDay:
                    startTime = series.StartTime.FloorDay();
                    offsetDuration = series.StartTime.CeilDay() - series.StartTime;
                    break;
                case DemandAggregation.OneWeek:
                    startTime = series.StartTime.FloorWeek();
                    offsetDuration = series.StartTime.CeilWeek() - series.StartTime;
                    break;
                default:
                    return series;
            }

            int chunkSize = (int)aggregation;
            int offset = (int)(offsetDuration / TimeSpan.FromMinutes(15));

            var aggregatedValues = series.Values()
                .Chunk(chunkSize, offset)
                .Select(chunk =>
                {
                    return chunk.Max();
                })
                .ToArray();

            var interval = chunkSize * TimeSpan.FromMinutes(15);
            return new FixedIntervalTimeSeries<float>(aggregatedValues, startTime, interval);
        }
    }
}
