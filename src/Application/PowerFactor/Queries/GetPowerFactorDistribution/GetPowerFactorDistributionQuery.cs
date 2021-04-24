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

            var emQuantities = CreateQuantities(phases.ToArray());

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

        public ElectricityMeterQuantity[] CreateQuantities(Phase[] phases)
        {
            var emQuantityTypes = new ElectricityMeterQuantityType[] {
                    ElectricityMeterQuantityType.ActiveEnergy,
                    ElectricityMeterQuantityType.ReactiveEnergyL,
                    ElectricityMeterQuantityType.ReactiveEnergyC,
                };

            var quanities = new List<ElectricityMeterQuantity>();

            foreach (var qt in emQuantityTypes)
            {
                foreach (var p in phases)
                {
                    quanities.Add(new ElectricityMeterQuantity
                    {
                        Type = qt,
                        Phase = p
                    });
                }
            }

            return quanities.ToArray();
        }

        public float[] CalcCosFiForPhase(ElectricityMeterRowsView emView, Phase phase)
        {
            var activeEnergy = emView.GetDifferenceInQuarterHours(new ElectricityMeterQuantity
            {
                Type = ElectricityMeterQuantityType.ActiveEnergy,
                Phase = phase,
            });
            var reactiveEnergyL = emView.GetDifferenceInQuarterHours(new ElectricityMeterQuantity
            {
                Type = ElectricityMeterQuantityType.ReactiveEnergyL,
                Phase = phase,
            });
            var reactiveEnergyC = emView.GetDifferenceInQuarterHours(new ElectricityMeterQuantity
            {
                Type = ElectricityMeterQuantityType.ReactiveEnergyC,
                Phase = phase,
            });

            var ep = activeEnergy.Values().ToArray();
            var eqL = reactiveEnergyL.Values().ToArray();
            var eqC = reactiveEnergyC.Values().ToArray();
            var cosFi = new float[ep.Length];

            for (int i = 0; i < ep.Length; i++)
            {
                cosFi[i] = ElectricityUtil.CalcCosFi(ep[i], eqL[i] - eqC[i]);
            }

            return cosFi;
        }

        public int[] BinValues(float[] values, float[] thresholds)
        {
            int n = thresholds.Length + 1;
            var bins = new int[n];
            Array.Fill(bins, 0);

            foreach (var v in values)
            {
                int i = 0;
                for (; i < (n - 2); i++)
                {
                    if (v < thresholds[i])
                    {
                        bins[i] += 1;
                        break;
                    }
                }

                // last threshold is inclusive
                if (v <= thresholds[i])
                {
                    bins[i] += 1;
                    continue;
                }

                bins[i + 1] += 1;
            }

            return bins;
        }

        public Tuple<BinRange, int>[] CreateDistributionTuples(int[] bins, float[] thresholds)
        {
            var items = new List<Tuple<BinRange, int>>();

            items.Add(Tuple.Create(new BinRange
            {
                Start = null,
                End = thresholds[0]
            }, bins[0]));

            int i = 1;
            for (; i < thresholds.Length; i++)
            {
                items.Add(Tuple.Create(new BinRange
                {
                    Start = thresholds[i - 1],
                    End = thresholds[i]
                }, bins[i]));
            }

            items.Add(Tuple.Create(new BinRange
            {
                Start = null,
                End = thresholds[0]
            }, bins[i]));

            return items.ToArray();
        }

        string CreateBinKey(float start, float end)
        {
            const float RES = 0.001f;
            return start.ToString("0.000") + "-" + (end - RES).ToString("0.000");
        }


        public Dictionary<string, int> CalcGeneralDistribution(float[] cosFi)
        {
            var counter = new Dictionary<string, int>();
            counter.Add("0.950-1.000", 0);
            counter.Add("0.900-0.949", 0);
            counter.Add("0.800-0.899", 0);
            counter.Add("0.700-0.799", 0);
            counter.Add("0.600-0.699", 0);
            counter.Add("0.000-0.599", 0);
            counter.Add("outlier", 0);

            foreach (float value in cosFi)
            {
                float absVal = Math.Abs(value);
                switch (absVal)
                {
                    case var v when v <= 1 && v >= 0.95:
                        counter["0.950-1.000"] += 1;
                        break;
                    case var v when v >= 0.9:
                        counter["0.900-0.949"] += 1;
                        break;
                    case var v when v >= 0.8:
                        counter["0.800-0.899"] += 1;
                        break;
                    case var v when v >= 0.7:
                        counter["0.700-0.799"] += 1;
                        break;
                    case var v when v >= 0.6:
                        counter["0.600-0.699"] += 1;
                        break;
                    case var v when v >= 0:
                        counter["0.000-0.599"] += 1;
                        break;
                    default:
                        counter["outlier"] += 1;
                        break;
                }
            }

            return counter;
        }
    }
}