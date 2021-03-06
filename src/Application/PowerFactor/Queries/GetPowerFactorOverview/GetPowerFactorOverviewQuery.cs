using AutoMapper;
using DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Common.Models.Queries;
using Electricity.Application.Common.Services;
using Electricity.Application.Common.Utils;
using MediatR;
using Newtonsoft.Json;
using NJsonSchema.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.PowerFactor.Queries.GetPowerFactorOverview
{
    public class GetPowerFactorOverviewQuery : IRequest<PowerFactorOverviewDto>
    {
        public IntervalDto Interval1 { get; set; }

        public IntervalDto? Interval2 { get; set; }

        public Guid[] GroupIds { get; set; }
    }

    public class GetPowerFactorOverviewQueryHandler : IRequestHandler<GetPowerFactorOverviewQuery, PowerFactorOverviewDto>
    {
        private readonly ElectricityMeterService _electricityMeterService;
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;

        public GetPowerFactorOverviewQueryHandler(
            ElectricityMeterService electricityMeterService,
            IGroupService groupService,
            IMapper mapper)
        {
            _electricityMeterService = electricityMeterService;
            _groupService = groupService;
            _mapper = mapper;
        }

        public Task<PowerFactorOverviewDto> Handle(GetPowerFactorOverviewQuery request, CancellationToken cancellationToken)
        {
            var interval1 = _mapper.Map<Interval>(request.Interval1);
            var interval2 = _mapper.Map<Interval>(request.Interval2);

            var userGroups = _groupService.GetUserGroups();
            if (userGroups.Length == 0) return null;

            var items1 = GetItemsForInterval(userGroups, interval1, nameof(request.Interval1));
            var items2 = GetItemsForInterval(userGroups, interval2, nameof(request.Interval2));

            return Task.FromResult(new PowerFactorOverviewDto
            {
                Items1 = items1,
                Items2 = items2
            });
        }

        public PowerFactorOverviewItem[] GetItemsForInterval(Group[] groups, Interval interval, string intervalName)
        {
            if (interval == null) return null;

            var items = groups.Select(g =>
            {
                var emQuantities = new ElectricityMeterQuantity[] {
                    ElectricityMeterQuantity.ActiveEnergy,
                    ElectricityMeterQuantity.ReactiveEnergyL,
                    ElectricityMeterQuantity.ReactiveEnergyC,
                };

                var emView = _electricityMeterService.GetRowsView(g.ID, interval, emQuantities);
                if (emView == null)
                {
                    throw new IntervalOutOfRangeException(intervalName);
                }

                var activeEnergy = emView.GetDifference(ElectricityMeterQuantity.ActiveEnergy);
                var reactiveEnergyL = emView.GetDifference(ElectricityMeterQuantity.ReactiveEnergyL);
                var reactiveEnergyC = emView.GetDifference(ElectricityMeterQuantity.ReactiveEnergyC);

                var cosFi = ElectricityUtil.CalcCosFi(activeEnergy, reactiveEnergyL - reactiveEnergyC);

                return new PowerFactorOverviewItem
                {
                    GroupName = g.Name,
                    ActiveEnergy = activeEnergy,
                    ReactiveEnergyL = reactiveEnergyL,
                    ReactiveEnergyC = reactiveEnergyC,
                    CosFi = cosFi,
                    Interval = emView.GetInterval()
                };
            });

            return items.ToArray();
        }
    }
}