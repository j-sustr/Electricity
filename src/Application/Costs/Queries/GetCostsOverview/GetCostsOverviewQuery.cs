using AutoMapper;
using KMB.DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Exceptions;
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

namespace Electricity.Application.Costs.Queries.GetCostsOverview
{
    public class GetCostsOverviewQuery : IRequest<CostsOverviewDto>
    {
        public IntervalDto Interval1 { get; set; }

        public IntervalDto? Interval2 { get; set; }
    }

    public class GetCostsOverviewQueryHandler : IRequestHandler<GetCostsOverviewQuery, CostsOverviewDto>
    {
        private readonly ElectricityMeterService _electricityMeterService;
        private readonly PowerService _powerService;
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;

        public GetCostsOverviewQueryHandler(
            ElectricityMeterService electricityMeterService,
            PowerService powerService,
            IGroupService groupService,
            IMapper mapper)
        {
            _electricityMeterService = electricityMeterService;
            _powerService = powerService;
            _groupService = groupService;
            _mapper = mapper;
        }

        public Task<CostsOverviewDto> Handle(GetCostsOverviewQuery request, CancellationToken cancellationToken)
        {
            var interval1 = _mapper.Map<Interval>(request.Interval1);
            var interval2 = _mapper.Map<Interval>(request.Interval2);

            var userGroups = _groupService.GetUserGroups();
            if (userGroups.Length == 0) return null;

            var items1 = GetItemsForInterval(userGroups, interval1, nameof(request.Interval1));
            var items2 = GetItemsForInterval(userGroups, interval2, nameof(request.Interval2));

            return Task.FromResult(new CostsOverviewDto
            {
                Items1 = items1,
                Items2 = items2,
            });
        }

        public CostlyQuantitiesOverviewItem[] GetItemsForInterval(Group[] groups, Interval interval, string intervalName)
        {
            if (interval == null) return null;

            var items = groups.Select(g =>
            {
                var emQuantities = new ElectricityMeterQuantity[] {
                    new ElectricityMeterQuantity{
                        Type = ElectricityMeterQuantityType.ActiveEnergy,
                        Phase = Phase.Main
                    },
                    new ElectricityMeterQuantity{
                        Type = ElectricityMeterQuantityType.ReactiveEnergyL,
                        Phase = Phase.Main
                    }
                };

                var powQuantities = new PowerQuantity[] {
                    new PowerQuantity
                    {
                        Type = PowerQuantityType.PAvg3P,
                        Phase = Phase.Main
                    }
                };

                var emView = _electricityMeterService.GetRowsView(g.ID, interval, emQuantities);
                var powView = _powerService.GetRowsView(g.ID, interval, powQuantities);
                if (emView == null || powView == null)
                {
                    throw new IntervalOutOfRangeException(intervalName);
                }
                //var emInterval = emView.GetInterval();
                //var powInterval = powView.GetInterval();
                //if (!interval.Equals(emInterval) || !interval.Equals(powInterval))
                //{
                //    throw new IntervalOutOfRangeException(intervalName);
                //}

                var activeEnergy = emView.GetDifferenceInMonths(new ElectricityMeterQuantity
                {
                    Type = ElectricityMeterQuantityType.ActiveEnergy,
                    Phase = Phase.Main
                });
                var reactiveEnergyL = emView.GetDifferenceInMonths(new ElectricityMeterQuantity
                {
                    Type = ElectricityMeterQuantityType.ReactiveEnergyL,
                    Phase = Phase.Main
                });
                var peakDemand = powView.GetPeakDemandInMonths(new PowerQuantity
                {
                    Type = PowerQuantityType.PAvg3P,
                    Phase = Phase.Main
                });

                return new CostlyQuantitiesOverviewItem
                {
                    GroupId = g.ID.ToString(),
                    GroupName = g.Name,

                    ActiveEnergyInMonths = activeEnergy.Values().ToArray(),
                    ReactiveEnergyInMonths = reactiveEnergyL.Values().ToArray(),
                    PeakDemandInMonths = peakDemand.Values().ToArray()
                };
            });

            return items.ToArray();
        }
    }
}