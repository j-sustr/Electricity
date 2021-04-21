using AutoMapper;
using KMB.DataSource;
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

        public int? MaxGroups { get; set; }
    }

    public class GetPowerFactorOverviewQueryHandler : IRequestHandler<GetPowerFactorOverviewQuery, PowerFactorOverviewDto>
    {
        private readonly ElectricityMeterService _electricityMeterService;
        private readonly IGroupRepository _groupService;
        private readonly IMapper _mapper;

        public GetPowerFactorOverviewQueryHandler(
            ElectricityMeterService electricityMeterService,
            IGroupRepository groupService,
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

            var recordGroupInfos = _groupService.GetUserRecordGroupInfos();
            if (recordGroupInfos.Length == 0) 
                return Task.FromResult<PowerFactorOverviewDto>(null);
            if (request.MaxGroups is int max)
            {
                recordGroupInfos = recordGroupInfos.Take(max).ToArray();
            }

            var items1 = GetItemsForInterval(recordGroupInfos, interval1, nameof(request.Interval1));
            var items2 = GetItemsForInterval(recordGroupInfos, interval2, nameof(request.Interval2));

            return Task.FromResult(new PowerFactorOverviewDto
            {
                Items1 = items1,
                Items2 = items2
            });
        }

        public PowerFactorOverviewItem[] GetItemsForInterval(GroupInfo[] groups, Interval interval, string intervalName)
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
                    },
                    new ElectricityMeterQuantity{
                        Type = ElectricityMeterQuantityType.ReactiveEnergyC,
                        Phase = Phase.Main
                    }
                };

                var emView = _electricityMeterService.GetRowsView(g.ID, interval, emQuantities);
                if (emView == null)
                {
                    throw new IntervalOutOfRangeException(intervalName);
                }

                var activeEnergy = emView.GetDifference(new ElectricityMeterQuantity
                {
                    Type = ElectricityMeterQuantityType.ActiveEnergy,
                    Phase = Phase.Main
                });
                var reactiveEnergyL = emView.GetDifference(new ElectricityMeterQuantity
                {
                    Type = ElectricityMeterQuantityType.ReactiveEnergyL,
                    Phase = Phase.Main
                });
                var reactiveEnergyC = emView.GetDifference(new ElectricityMeterQuantity
                {
                    Type = ElectricityMeterQuantityType.ReactiveEnergyC,
                    Phase = Phase.Main
                });

                var cosFi = ElectricityUtil.CalcCosFi(activeEnergy, reactiveEnergyL - reactiveEnergyC);

                return new PowerFactorOverviewItem
                {
                    GroupId = g.ID.ToString(),
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