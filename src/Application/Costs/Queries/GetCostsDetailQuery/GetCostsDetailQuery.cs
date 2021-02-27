using AutoMapper;
using DataSource;
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

namespace Electricity.Application.Costs.Queries.GetCostsDetailQuery
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

            var items1 = GetItemsForInterval(group, interval1);
            var items2 = GetItemsForInterval(group, interval2);

            return Task.FromResult(new CostsDetailDto
            {
                Items1 = items1,
                Items2 = items2,
            });
        }

        public CostsDetailItem[] GetItemsForInterval(Group g, Interval interval)
        {
            if (interval == null)
            {
                return null;
            }

            var powInterval = _electricityMeterService.GetIntervalOverlap(g.ID, interval);
            var emInterval = _powerService.GetIntervalOverlap(g.ID, interval);
            if (!powInterval.Equals(emInterval))
            {
                return null;
            }

            var emQuantities = new ElectricityMeterQuantity[] {
                    ElectricityMeterQuantity.ActiveEnergy,
                    ElectricityMeterQuantity.ReactiveEnergyL
                };

            var powQuantities = new PowerQuantity[] {
                    PowerQuantity.PAvg3P
                };

            var emView = _electricityMeterService.GetRowsView(g.ID, interval, emQuantities);
            var powView = _powerService.GetRowsView(g.ID, interval, powQuantities);

            var activeEnergy = emView.GetDifferenceInMonths(ElectricityMeterQuantity.ActiveEnergy);
            var reactiveEnergyL = emView.GetDifferenceInMonths(ElectricityMeterQuantity.ReactiveEnergyL);
            var peakDemand = powView.GetPeakDemandInMonths(PowerQuantity.PAvg3P);

            var items = new List<CostsDetailItem>();
            for (int i = 0; i < activeEnergy.Size; i++)
            {
                var time = activeEnergy.TimeAt(i);
                var item = new CostsDetailItem
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