using AutoMapper;
using DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Common.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.Costs.Queries.GetCostsDetail
{
    public class GetCostsDetailQuery : IRequest<CostsDetailDto>
    {
        public IntervalDto Interval1 { get; set; }

        public IntervalDto? Interval2 { get; set; }

        public string GroupId { get; set; }
    }

    public class GetCostsDetailQueryHandler : IRequestHandler<GetCostsDetailQuery, CostsDetailDto>
    {
        private readonly ElectricityMeterService _electricityMeterService;
        private readonly PowerService _powerService;
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;

        public GetCostsDetailQueryHandler(
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

        public Task<CostsDetailDto> Handle(GetCostsDetailQuery request, CancellationToken cancellationToken)
        {
            var interval1 = _mapper.Map<Interval>(request.Interval1);
            var interval2 = _mapper.Map<Interval>(request.Interval2);

            var group = _groupService.GetGroupById(request.GroupId);
            if (group == null)
            {
                throw new NotFoundException("Invalid GroupId");
            }

            var items1 = GetItemsForInterval(group, interval1, nameof(request.Interval1));
            var items2 = GetItemsForInterval(group, interval2, nameof(request.Interval2));

            return Task.FromResult(new CostsDetailDto
            {
                Items1 = items1,
                Items2 = items2,
            });
        }

        public CostlyQuantitiesDetailItem[] GetItemsForInterval(Group g, Interval interval, string intervalName)
        {
            if (interval == null)
            {
                return null;
            }

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
                    new PowerQuantity{
                        Type = PowerQuantityType.PAvg3P
                    }
                };

            var emView = _electricityMeterService.GetRowsView(g.ID, interval, emQuantities);
            var powView = _powerService.GetRowsView(g.ID, interval, powQuantities);
            if (emView == null || powView == null)
            {
                throw new IntervalOutOfRangeException(intervalName);
            }

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
                Type = PowerQuantityType.PAvg3P
            });

            var items = new List<CostlyQuantitiesDetailItem>();
            for (int i = 0; i < activeEnergy.Size; i++)
            {
                var time = activeEnergy.TimeAt(i);
                var item = new CostlyQuantitiesDetailItem
                {
                    Year = time.Year,
                    Month = time.Month,

                    ActiveEnergy = activeEnergy.ValueAt(i),
                    ReactiveEnergy = reactiveEnergyL.ValueAt(i),
                    PeakDemand = peakDemand.ValueAt(i)
                };
                items.Add(item);
            }

            return items.ToArray();
        }
    }
}