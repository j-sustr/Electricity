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
    }

    public class GetPowerFactorDistributionQueryHandler : IRequestHandler<GetPowerFactorDistributionQuery, PowerFactorDistributionDto>
    {
        private readonly ElectricityMeterService _electricityMeterService;
        private readonly PowerService _powerService;
        private readonly IGroupRepository _groupService;
        private readonly IMapper _mapper;

        public GetPowerFactorDistributionQueryHandler(
            ElectricityMeterService electricityMeterService,
            PowerService powerService,
            IGroupRepository groupService,
            IMapper mapper)
        {
            _electricityMeterService = electricityMeterService;
            _powerService = powerService;
            _groupService = groupService;
            _mapper = mapper;
        }

        public Task<PowerFactorDistributionDto> Handle(GetPowerFactorDistributionQuery request, CancellationToken cancellationToken)
        {
            var interval1 = _mapper.Map<Interval>(request.Interval1);
            var interval2 = _mapper.Map<Interval>(request.Interval2);
            var phases = request.Phases;

            var group = _groupService.GetGroupInfo(request.GroupId);
            if (group == null)
            {
                throw new NotFoundException("Invalid GroupId");
            }

            var items1 = GetItemsForInterval(group, interval1, phases, nameof(request.Interval1));
            var items2 = GetItemsForInterval(group, interval2, phases, nameof(request.Interval2));

            return Task.FromResult(new PowerFactorDistributionDto
            {
                GroupName = group.Name,
                Distribution1 = items1,
                Distribution2 = items2,
            });
        }

        public PowerFactorDistributionItem[] GetItemsForInterval(GroupInfo g, Interval interval, Phases phases, string intervalName)
        {
            if (interval == null)
            {
                return null;
            }

            var emQuantities = CreateQuantities(phases.ToArray());

            var emView = _electricityMeterService.GetRowsView(g.ID, interval, emQuantities);
            if (emView == null)
            {
                throw new IntervalOutOfRangeException(intervalName);
            }

            var distributions = new Dictionary<Phase, Dictionary<string, int>>();

            foreach (var phase in phases.ToArray())
            {
                distributions[phase] = CalcPowerFactorDistributionForPhase(emView, phase);
            }

            return DistributionToItems(distributions, phases);
        }

        public PowerFactorDistributionItem[] DistributionToItems(Dictionary<Phase, Dictionary<string, int>> distributions, Phases phases)
        {
            var items = new List<PowerFactorDistributionItem>();

            var dist = distributions[phases.ToArray()[0]];

            foreach (var entry in dist)
            {
                var item = new PowerFactorDistributionItem();

                item.Range = entry.Key;

                if (phases.Main == true)
                    item.ValueMain = distributions[Phase.Main][entry.Key];
                if (phases.L1 == true)
                    item.ValueL1 = distributions[Phase.L1][entry.Key];
                if (phases.L2 == true)
                    item.ValueL2 = distributions[Phase.L2][entry.Key];
                if (phases.L3 == true)
                    item.ValueL3 = distributions[Phase.L3][entry.Key];

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

        public Dictionary<string, int> CalcPowerFactorDistributionForPhase(ElectricityMeterRowsView emView, Phase phase)
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

            return CalcPowerFactorDistribution(cosFi);
        }

        public Dictionary<string, int> CalcPowerFactorDistribution(float[] cosFi)
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