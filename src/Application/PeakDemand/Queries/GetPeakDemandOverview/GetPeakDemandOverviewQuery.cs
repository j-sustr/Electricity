using AutoMapper;
using KMB.DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Common.Services;
using Electricity.Application.Costs.Queries.GetCostsOverview;
using MediatR;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.PeakDemand.Queries.GetPeakDemandOverview
{
    public class GetPeakDemandOverviewQuery : IRequest<PeakDemandOverviewDto>
    {
        public IntervalDto Interval1 { get; set; }

        public IntervalDto? Interval2 { get; set; }
    }

    public class GetPeakDemandDetailQueryHandler : IRequestHandler<GetPeakDemandOverviewQuery, PeakDemandOverviewDto>
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

        public Task<PeakDemandOverviewDto> Handle(GetPeakDemandOverviewQuery request, CancellationToken cancellationToken)
        {
            var interval1 = _mapper.Map<Interval>(request.Interval1);
            var interval2 = _mapper.Map<Interval>(request.Interval2);

            var userRecordGroups = _groupService.GetUserRecordGroupInfos();
            if (userRecordGroups.Length == 0) return null;

            var items1 = GetItemsForInterval(userRecordGroups, interval1, nameof(request.Interval1));
            var items2 = GetItemsForInterval(userRecordGroups, interval2, nameof(request.Interval2));

            return Task.FromResult(new PeakDemandOverviewDto
            {
                Items1 = items1,
                Items2 = items2,
            });
        }

        private PeakDemandOverviewItem[] GetItemsForInterval(GroupInfo[] groups, Interval interval, string intervalName)
        {
            if (interval == null) return null;

            var items = groups.Select(g =>
            {
                var powQuantities = new PowerQuantity[] {
                    new PowerQuantity
                    {
                        Type = PowerQuantityType.PAvg3P,
                        Phase = Phase.Main
                    }
                };

                var powView = _powerService.GetRowsView(g.ID, interval, powQuantities);
                if (powView == null)
                {
                    throw new IntervalOutOfRangeException(intervalName);
                }

                var peakDemandInMonths = powView.GetPeakDemandInMonths(new PowerQuantity
                {
                    Type = PowerQuantityType.PAvg3P,
                    Phase = Phase.Main
                });

                var peakDemand = peakDemandInMonths.Entries()
                    .MaxBy(ent => ent.Item2).Take(1).FirstOrDefault();

                return new PeakDemandOverviewItem
                {
                    GroupId = g.ID.ToString(),
                    GroupName = g.Name,

                    PeakDemandTime = peakDemand.Item1,
                    PeakDemandValue = peakDemand.Item2
                };
            });

            return items.ToArray();
        }
    }
}