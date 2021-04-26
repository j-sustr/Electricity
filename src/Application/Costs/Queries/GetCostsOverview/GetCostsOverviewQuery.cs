﻿using AutoMapper;
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
using Electricity.Application.Common.Utils;

namespace Electricity.Application.Costs.Queries.GetCostsOverview
{
    public class GetCostsOverviewQuery : IRequest<CostsOverviewDto>
    {
        public IntervalDto Interval1 { get; set; }

        public IntervalDto? Interval2 { get; set; }

        public int? MaxGroups { get; set; }
    }

    public class GetCostsOverviewQueryHandler : IRequestHandler<GetCostsOverviewQuery, CostsOverviewDto>
    {
        private readonly ArchiveRepositoryService _archiveRepoService;
        private readonly IGroupRepository _groupService;
        private readonly IMapper _mapper;

        public GetCostsOverviewQueryHandler(
            ArchiveRepositoryService archiveRepoService,            
            IGroupRepository groupService,
            IMapper mapper)
        {
            _archiveRepoService = archiveRepoService;
            _groupService = groupService;
            _mapper = mapper;
        }

        public Task<CostsOverviewDto> Handle(GetCostsOverviewQuery request, CancellationToken cancellationToken)
        {
            var interval1 = _mapper.Map<Interval>(request.Interval1);
            var interval2 = _mapper.Map<Interval>(request.Interval2);

            var recordGroupInfos = _groupService.GetUserRecordGroupInfos();
            if (recordGroupInfos.Length == 0) 
                return Task.FromResult<CostsOverviewDto>(null);
            if (request.MaxGroups is int max)
            {
                recordGroupInfos = recordGroupInfos.Take(max).ToArray();
            }

            var items1 = GetItemsForInterval(recordGroupInfos, interval1, nameof(request.Interval1));
            var items2 = GetItemsForInterval(recordGroupInfos, interval2, nameof(request.Interval2));

            return Task.FromResult(new CostsOverviewDto
            {
                Items1 = items1,
                Items2 = items2,
            });
        }

        public CostlyQuantitiesOverviewItem[] GetItemsForInterval(GroupInfo[] groupInfos, Interval interval, string intervalName)
        {
            if (interval == null) return null;

            var items = groupInfos.Select(g =>
            {
                bool hasMainArch = _archiveRepoService.HasArchive(g.ID, Arch.Main);
                bool hasEMArch = _archiveRepoService.HasArchive(g.ID, Arch.ElectricityMeter);
                if (!hasMainArch || !hasEMArch)
                {
                    return new CostlyQuantitiesOverviewItem
                    {
                        Message = ArchiveUtils.CreateMissingArchivesMessage(!hasMainArch, !hasEMArch)
                    };
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
                    new PowerQuantity
                    {
                        Type = PowerQuantityType.PAvg,
                        Phase = Phase.Main
                    }
                };

                var emView = _archiveRepoService.GetElectricityMeterRowsView(new GetElectricityMeterRowsViewQuery
                {
                    GroupId = g.ID,
                    Range = interval,
                    Quantities = emQuantities
                });
                var powView = _archiveRepoService.GetPowerRowsView(new GetPowerRowsViewQuery
                {
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
                var peakDemandValues = peakDemand.Select(pd => pd.Value);

                return new CostlyQuantitiesOverviewItem
                {
                    GroupId = g.ID.ToString(),
                    GroupName = g.Name,

                    ActiveEnergyInMonths = activeEnergy.Values().ToArray(),
                    ReactiveEnergyInMonths = reactiveEnergyL.Values().ToArray(),
                    PeakDemandInMonths = peakDemandValues.ToArray()
                };
            });

            return items.ToArray();
        }
    }
}