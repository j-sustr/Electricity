using AutoMapper;
using KMB.DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Dtos;
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
using Electricity.Application.Common.Constants;

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
        private readonly ArchiveRepositoryService _archiveRepoService;
        private readonly IGroupRepository _groupService;
        private readonly IMapper _mapper;

        public GetPowerFactorOverviewQueryHandler(
            ArchiveRepositoryService archiveRepoService,
            IGroupRepository groupService,
            IMapper mapper)
        {
            _archiveRepoService = archiveRepoService;
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
                bool hasMainArch = _archiveRepoService.HasArchive(g.ID, Arch.Main);
                bool hasEMArch = _archiveRepoService.HasArchive(g.ID, Arch.ElectricityMeter);
                if (!hasMainArch || !hasEMArch)
                {
                    return new PowerFactorOverviewItem
                    {
                        GroupId = g.ID.ToString(),
                        GroupName = g.Name,
                        Message = ArchiveUtils.CreateMissingArchivesMessage(!hasMainArch, !hasEMArch)
                    };
                }

                var subinterval = _archiveRepoService.GetRangeOverlapWithElectrityMeter(g.ID, interval);
                if (subinterval == null)
                {
                    bool hasDataMain = _archiveRepoService.HasDataOnRange(g.ID, interval, Arch.Main);
                    bool hasDataEM = _archiveRepoService.HasDataOnRange(g.ID, interval, Arch.ElectricityMeter);
                    return new PowerFactorOverviewItem
                    {
                        GroupId = g.ID.ToString(),
                        GroupName = g.Name,
                        Message = ArchiveUtils.CreateArchivesDoNotHaveDataOnRangeMessage(!hasDataMain, !hasDataEM)
                    };
                }
                interval = subinterval;

                var mainQuantities = new MainQuantity[]
                {
                    new MainQuantity
                    {
                        Type = MainQuantityType.CosFi,
                        Phase = Phase.Main
                    }
                };
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
                    },
                };
                var mainView = _archiveRepoService.GetMainRowsView(new GetMainRowsViewQuery
                {
                    GroupId = g.ID,
                    Range = interval,
                    Quantities = mainQuantities,
                    Aggregation = ApplicationConstants.MAIN_AGGREGATION
                });
                var emView = _archiveRepoService.GetElectricityMeterRowsView(new GetElectricityMeterRowsViewQuery {
                    GroupId = g.ID,
                    Range = interval,
                    Quantities = emQuantities,
                    Aggregation = ApplicationConstants.EM_AGGREGATION,
                    EnergyAggType = EEnergyAggType.Cumulative
                });

                var activeEnergy = emView.GetTotalDifference(new ElectricityMeterQuantity
                {
                    Type = ElectricityMeterQuantityType.ActiveEnergy,
                    Phase = Phase.Main
                });
                var reactiveEnergyL = emView.GetTotalDifference(new ElectricityMeterQuantity
                {
                    Type = ElectricityMeterQuantityType.ReactiveEnergyL,
                    Phase = Phase.Main
                });
                var reactiveEnergyC = emView.GetTotalDifference(new ElectricityMeterQuantity
                {
                    Type = ElectricityMeterQuantityType.ReactiveEnergyC,
                    Phase = Phase.Main
                });
                var cosFi = mainView.GetColumnValues(new MainQuantity
                {
                    Type = MainQuantityType.CosFi,
                    Phase = Phase.Main
                })
                .ToArray()
                .Average();

                return new PowerFactorOverviewItem
                {
                    GroupId = g.ID.ToString(),
                    GroupName = g.Name,
                    Interval = _mapper.Map<IntervalDto>(interval),

                    ActiveEnergy = activeEnergy,
                    ReactiveEnergyL = reactiveEnergyL,
                    ReactiveEnergyC = reactiveEnergyC,
                    CosFi = cosFi,
                };
            });

            return items.ToArray();
        }
    }
}