using AutoMapper;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Common.Services;
using Electricity.Application.PeakDemand.Queries.GetPeakDemandOverview;
using KMB.DataSource;
using MediatR;
using MoreLinq;
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

            var data1 = GetDataForInterval(groupInfo, interval1, nameof(request.Interval1));
            var data2 = GetDataForInterval(groupInfo, interval2, nameof(request.Interval2));

            return Task.FromResult(new PeakDemandDetailDto
            {
                Data1 = data1,
                Data2 = data2,
            });
        }

        private PeakDemandDetailData GetDataForInterval(GroupInfo group, Interval interval, string intervalName)
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

            var q = new PowerQuantity
            {
                Type = PowerQuantityType.PAvg3P,
                Phase = Phase.Main
            };
            var demandSeries = powView.GetDemandSeries(q);

            return new PeakDemandDetailData
            {
                DemandSeries = demandSeries.Entries().Select(ent => {
                    return new object[2] { ent.Item1, ent.Item2 };
                }).ToArray()
            };
        }
    }
}
