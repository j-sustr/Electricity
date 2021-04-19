using AutoMapper;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Exceptions;
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

        public int Aggregation { get; set; } // 0 - none, 1 - 1hour, 2 - 6hours, 3 - 12hours,  4 - 24hours, 5 - 7days

    }

    public class GetPeakDemandDetailQueryHandler : IRequestHandler<GetPeakDemandDetailQuery, PeakDemandDetailDto>
    {
        private readonly PowerService _powerService;
        private readonly IGroupRepository _groupService;
        private readonly IMapper _mapper;

        public GetPeakDemandDetailQueryHandler(
            PowerService powerService,
            IGroupRepository groupService,
            IMapper mapper)
        {
            _powerService = powerService;
            _groupService = groupService;
            _mapper = mapper;
        }

        public Task<PeakDemandDetailDto> Handle(GetPeakDemandDetailQuery request, CancellationToken cancellationToken)
        {
            var interval1 = _mapper.Map<Interval>(request.Interval1);
            var interval2 = _mapper.Map<Interval>(request.Interval2);

            var groupInfo = _groupService.GetGroupInfo(request.GroupId);
            if (groupInfo == null) return null;

            var demandSeries1 = GetDemandSeriesForInterval(groupInfo, interval1, nameof(request.Interval1), request.Aggregation);
            var demandSeries2 = GetDemandSeriesForInterval(groupInfo, interval2, nameof(request.Interval2), request.Aggregation);

            return Task.FromResult(new PeakDemandDetailDto
            {
                DemandSeries1 = demandSeries1,
                DemandSeries2 = demandSeries2
            });
        }

        private DemandSeriesDto GetDemandSeriesForInterval(GroupInfo group, Interval interval, string intervalName, int aggregation)
        {
            if (interval == null) return null;

            var powQuantities = new PowerQuantity[] {
                    new PowerQuantity
                    {
                        Type = PowerQuantityType.PAvg3P,
                        Phase = Phase.Main
                    }
                };

            var powView = _powerService.GetRowsView(group.ID, interval, powQuantities);
            if (powView == null)
            {
                throw new IntervalOutOfRangeException(intervalName);
            }
            var resultInterval = powView.GetInterval();

            var q = new PowerQuantity
            {
                Type = PowerQuantityType.PAvg3P,
                Phase = Phase.Main
            };
            var seriesMain = powView.GetDemandSeries(q);
            if (aggregation > 0)
            {
                seriesMain = AggregateSeries(seriesMain, aggregation);
            }

            var valuesMain = seriesMain.Values().ToArray();

            return new DemandSeriesDto
            {
                TimeRange = _mapper.Map<IntervalDto>(resultInterval),
                TimeStep = (int)TimeSpan.FromMinutes(15).TotalMilliseconds, // get timestep from request.Aggregation
                ValuesMain = valuesMain
            };
        }

        public FixedIntervalTimeSeries<float> AggregateSeries(FixedIntervalTimeSeries<float> series, int aggregation)
        {
            int chunkSize;
            switch (aggregation)
            {
                case 1:
                    chunkSize = 4;
                    break;
                default:
                    return series;
            }

            throw new NotImplementedException();

            return series;
        }
    }
}
