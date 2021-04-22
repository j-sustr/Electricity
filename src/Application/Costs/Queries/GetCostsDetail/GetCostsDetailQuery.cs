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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Electricity.Application.Costs.Queries.GetCostsDetail
{
    public class GetCostsDetailQuery : IRequest<CostsDetailDto>
    {
        public string GroupId { get; set; }

        public IntervalDto Interval1 { get; set; }

        public IntervalDto? Interval2 { get; set; }
    }

    public class GetCostsDetailQueryHandler : IRequestHandler<GetCostsDetailQuery, CostsDetailDto>
    {
        private readonly ArchiveRepositoryService _archiveRepoService;
        private readonly IGroupRepository _groupService;
        private readonly IMapper _mapper;

        public GetCostsDetailQueryHandler(
            ArchiveRepositoryService archiveRepoService,
            IGroupRepository groupService,
            IMapper mapper)
        {
            _archiveRepoService = archiveRepoService;
            _groupService = groupService;
            _mapper = mapper;
        }

        public Task<CostsDetailDto> Handle(GetCostsDetailQuery request, CancellationToken cancellationToken)
        {
            var interval1 = _mapper.Map<Interval>(request.Interval1);
            var interval2 = _mapper.Map<Interval>(request.Interval2);

            var group = _groupService.GetGroupInfo(request.GroupId);
            if (group == null)
                throw new NotFoundException("group not found");

            var items1 = GetItemsForInterval(group, interval1, nameof(request.Interval1));
            var items2 = GetItemsForInterval(group, interval2, nameof(request.Interval2));

            return Task.FromResult(new CostsDetailDto
            {
                Items1 = items1,
                Items2 = items2,
            });
        }

        public CostlyQuantitiesDetailItem[] GetItemsForInterval(GroupInfo g, Interval interval, string intervalName)
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
                        Type = PowerQuantityType.PAvg,
                        Phase = Phase.Main
                    }
                };

            var emView = _archiveRepoService.GetElectricityMeterRowsView(new GetElectricityMeterRowsViewQuery {
                GroupId = g.ID, 
                Range = interval,
                Quantities = emQuantities
            });
            var powView = _archiveRepoService.GetPowerRowsView(new GetPowerRowsViewQuery {
                GroupId = g.ID,
                Range = interval,
                Quantities = powQuantities
            });

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
                Type = PowerQuantityType.PAvg,
                Phase = Phase.Main
            });
            var peakDemandValues = peakDemand.Select(pd => pd.Value).ToArray();

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
                    PeakDemand = peakDemandValues[i]
                };
                items.Add(item);
            }

            return items.ToArray();
        }
    }
}