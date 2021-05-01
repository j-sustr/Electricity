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
using Electricity.Application.Common.Constants;
using Electricity.Application.Common.Utils;

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

            var requiredArchives = new Arch[] { Arch.Main, Arch.ElectricityMeter };
            ArchiveUtils.MustHaveArchives(group, requiredArchives);
            interval1 = ArchiveUtils.MustGetSubintervalWithData(group, interval1, nameof(interval1), requiredArchives);
            interval2 = ArchiveUtils.MustGetSubintervalWithData(group, interval2, nameof(interval2), requiredArchives);

            var items1 = GetItemsForInterval(group, interval1, nameof(request.Interval1));
            var items2 = GetItemsForInterval(group, interval2, nameof(request.Interval2));

            return Task.FromResult(new CostsDetailDto
            {
                Items1 = items1,
                Items2 = items2,
                Interval1 = _mapper.Map<IntervalDto>(interval1),
                Interval2 = _mapper.Map<IntervalDto>(interval2)
            });
        }

        public CostlyQuantitiesDetailItem[] GetItemsForInterval(GroupInfo g, Interval interval, string intervalName)
        {
            if (interval == null) return null;

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

            var mainQuantities = new MainQuantity[] {
                    new MainQuantity{
                        Type = MainQuantityType.PAvg,
                        Phase = Phase.Main
                    },
                    new MainQuantity
                    {
                        Type = MainQuantityType.CosFi,
                        Phase = Phase.Main
                    },
                };

            var emView = _archiveRepoService.GetElectricityMeterRowsView(new GetElectricityMeterRowsViewQuery {
                GroupId = g.ID, 
                Range = interval,
                Quantities = emQuantities,
                Aggregation = ApplicationConstants.EM_AGGREGATION,
                EnergyAggType = EEnergyAggType.Cumulative
            });
            var mainView = _archiveRepoService.GetMainRowsView(new GetMainRowsViewQuery {
                GroupId = g.ID,
                Range = interval,
                Quantities = mainQuantities,
                Aggregation = ApplicationConstants.MAIN_AGGREGATION
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
            var peakDemand = mainView.GetPeakDemandInMonths(new MainQuantity
            {
                Type = MainQuantityType.PAvg,
                Phase = Phase.Main
            });
            var cosFi = mainView.GetCosFiInMonths(new MainQuantity
            {
                Type = MainQuantityType.CosFi,
                Phase = Phase.Main
            });
            var peakDemandValues = peakDemand.Select(pd => pd.Value).ToArray();
            var cosFiValues = cosFi.Select(entry => entry.Item2).ToArray();

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
                    PeakDemand = peakDemandValues[i],
                    CosFi = cosFiValues[i]
                };
                items.Add(item);
            }

            return items.ToArray();
        }
    }
}