using AutoMapper;
using KMB.DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Common.Services;
using Electricity.Application.Common.Utils;
using Electricity.Application.Costs.Queries.GetCostsDetail;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.PowerFactor.Queries.GetPowerFactorDistribution
{
    using static PowerFactorDistribution;

    public class GetPowerFactorDistributionQuery : IRequest<PowerFactorDistributionDto>
    {
        public string GroupId { get; set; }

        public IntervalDto Interval1 { get; set; }
        public IntervalDto? Interval2 { get; set; }

        public Phases Phases { get; set; }
        public float[] Thresholds { get; set; }
    }

    public class GetPowerFactorDistributionQueryHandler : IRequestHandler<GetPowerFactorDistributionQuery, PowerFactorDistributionDto>
    {
        readonly float[] DEFAULT_THRESHOLDS = new float[] { 0.000f, 0.600f, 0.700f, 0.800f, 0.900f, 0.950f, 1.0f };


        private readonly ArchiveRepositoryService _archiveRepoService;
        private readonly IGroupRepository _groupService;
        private readonly IMapper _mapper;

        public GetPowerFactorDistributionQueryHandler(
            ArchiveRepositoryService archiveRepoService,
            IGroupRepository groupService,
            IMapper mapper)
        {
            _archiveRepoService = archiveRepoService;
            _groupService = groupService;
            _mapper = mapper;
        }

        public Task<PowerFactorDistributionDto> Handle(GetPowerFactorDistributionQuery request, CancellationToken cancellationToken)
        {
            var group = _groupService.GetGroupInfo(request.GroupId);
            if (group == null)
                throw new NotFoundException("group not found");

            var interval1 = _mapper.Map<Interval>(request.Interval1);
            var interval2 = _mapper.Map<Interval>(request.Interval2);
            var phases = request.Phases;
            var thresholds = request.Thresholds;
            if (thresholds == null)
                thresholds = DEFAULT_THRESHOLDS;

            var items1 = GetItemsForInterval(group, interval1, phases, thresholds, nameof(request.Interval1));
            var items2 = GetItemsForInterval(group, interval2, phases, thresholds, nameof(request.Interval2));

            return Task.FromResult(new PowerFactorDistributionDto
            {
                GroupName = group.Name,
                Distribution1 = items1,
                Distribution2 = items2,
            });
        }

        public PowerFactorDistributionItem[] GetItemsForInterval(
            GroupInfo g, Interval interval, Phases phases, float[] thresholds, string intervalName)
        {
            if (interval == null) return null;

            var emQuantities = GetQuantities(phases.ToArray());

            var emView = _archiveRepoService.GetElectricityMeterRowsView(new GetElectricityMeterRowsViewQuery
            {
                GroupId = g.ID,
                Range = interval,
                Quantities = emQuantities
            });

            var distributions = new Dictionary<Phase, Tuple<BinRange, int>[]>();

            foreach (var phase in phases.ToArray())
            {
                var cosFi = CalcCosFiForPhase(emView, phase);
                var bins = BinValues(cosFi, thresholds);
                distributions[phase] = CreateDistributionTuples(bins, thresholds);
            }

            return DistributionToItems(distributions, phases);
        }

        public PowerFactorDistributionItem[] DistributionToItems(Dictionary<Phase, Tuple<BinRange, int>[]> distributions, Phases phases)
        {
            var items = new List<PowerFactorDistributionItem>();

            var entries = distributions[phases.ToArray()[0]];

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                var item = new PowerFactorDistributionItem();

                item.Range = entry.Item1;

                if (phases.Main == true)
                    item.ValueMain = distributions[Phase.Main][i].Item2;
                if (phases.L1 == true)
                    item.ValueL1 = distributions[Phase.L1][i].Item2;
                if (phases.L2 == true)
                    item.ValueL2 = distributions[Phase.L2][i].Item2;
                if (phases.L3 == true)
                    item.ValueL3 = distributions[Phase.L3][i].Item2;

                items.Add(item);
            }

            return items.ToArray();
        }

        
    }
}