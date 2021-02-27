using AutoMapper;
using DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Common.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.Costs.Queries.GetCostsQuery
{
    public class GetCostsOverviewQuery : IRequest<CostsOverviewDto>
    {
        public IntervalDto Interval1 { get; set; }

        public IntervalDto? Interval2 { get; set; }
    }

    public class GetCostsOverviewQueryHandler : IRequestHandler<GetCostsOverviewQuery, CostsOverviewDto>
    {
        private readonly ElectricityMeterService _electricityMeterService;
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;

        public GetCostsOverviewQueryHandler(
            ElectricityMeterService electricityMeterService,
            IGroupService groupService,
            IMapper mapper)
        {
            _electricityMeterService = electricityMeterService;
            _groupService = groupService;
            _mapper = mapper;
        }

        public Task<CostsOverviewDto> Handle(GetCostsOverviewQuery request, CancellationToken cancellationToken)
        {
            var interval1 = _mapper.Map<Interval>(request.Interval1);
            var interval2 = _mapper.Map<Interval>(request.Interval2);

            var userGroups = _groupService.GetUserGroups();

            var items1 = GetItemsForInterval(userGroups, interval1);
            var items2 = GetItemsForInterval(userGroups, interval2);

            return Task.FromResult(new CostsOverviewDto
            {
                Items1 = items1,
                Items2 = items2,
            });
        }

        public CostsOverviewItem[] GetItemsForInterval(Group[] groups, Interval interval)
        {
            if (groups == null || interval == null)
            {
                return null;
            }

            var items = groups.Select(g =>
            {
                var emQuantities = new ElectricityMeterQuantity[] {
                    ElectricityMeterQuantity.ActiveEnergy,
                    ElectricityMeterQuantity.ReactiveEnergyL
                };

                var emView = _electricityMeterService.GetRowsView(g.ID, interval, emQuantities);
                if (emView == null) return null;

                var rowsInterval = emView.GetInterval();

                var activeEnergy = emView.GetDifferenceInMonths(ElectricityMeterQuantity.ActiveEnergy);
                var reactiveEnergyL = emView.GetDifferenceInMonths(ElectricityMeterQuantity.ReactiveEnergyL);
                var peakDemand = emView.GetDifferenceInMonths(ElectricityMeterQuantity.ReactiveEnergyL);

                return new CostsOverviewItem
                {
                    GroupName = g.Name,

                    ActiveEnergyInMonths = activeEnergy,
                    ReactiveEnergyInMonths = reactiveEnergyL,
                    PeakDemandInMonths = peakDemand,
                    Interval = _mapper.Map<IntervalDto>(rowsInterval)
                };
            });

            return items.ToArray();
        }
    }
}