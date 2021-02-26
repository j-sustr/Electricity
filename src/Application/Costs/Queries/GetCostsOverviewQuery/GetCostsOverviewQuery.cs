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

            return Task.FromResult(new CostsOverviewDto
            {
                Interval1Items = GetItemsForInterval(userGroups, interval1),
                Interval2Items = GetItemsForInterval(userGroups, interval2),
            });
        }

        public CostsOverviewItem[] GetItemsForInterval(Group[] groups, Interval interval)
        {
            if (groups == null)
            {
                return null;
            }

            var items = groups.Select(g =>
            {
                var quantities = new ElectricityMeterQuantity[] {
                    ElectricityMeterQuantity.ActiveEnergy,
                };

                var view = _electricityMeterService.GetRowsView(g.ID, interval, quantities);
                if (view == null) return null;

                var rowsInterval = view.GetInterval();

                var activeEnergy = view.GetDifference(ElectricityMeterQuantity.ActiveEnergy);
                var reactiveEnergyL = view.GetDifference(ElectricityMeterQuantity.ReactiveEnergyL);

                return new CostsOverviewItem
                {
                    ActiveEnergy = activeEnergy,
                    ReactiveEnergy = reactiveEnergyL,
                    PeakDemand = 0
                };
            });

            return items.ToArray();
        }
    }
}