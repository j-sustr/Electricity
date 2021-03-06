using AutoMapper;
using DataSource;
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
    }

    public class GetPowerFactorDistributionQueryHandler : IRequestHandler<GetPowerFactorDistributionQuery, PowerFactorDistributionDto>
    {
        private readonly ElectricityMeterService _electricityMeterService;
        private readonly PowerService _powerService;
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;

        public GetPowerFactorDistributionQueryHandler(
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

        public Task<PowerFactorDistributionDto> Handle(GetPowerFactorDistributionQuery request, CancellationToken cancellationToken)
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

            return Task.FromResult(new PowerFactorDistributionDto
            {
                Distribution1 = items1,
                Distribution2 = items2,
            });
        }

        public PowerFactorDistributionItem[] GetItemsForInterval(Group g, Interval interval, string intervalName)
        {
            if (interval == null)
            {
                return null;
            }

            var emQuantities = new ElectricityMeterQuantity[] {
                    ElectricityMeterQuantity.ActiveEnergy,
                    ElectricityMeterQuantity.ReactiveEnergyL,
                    ElectricityMeterQuantity.ReactiveEnergyC,
                };

            var emView = _electricityMeterService.GetRowsView(g.ID, interval, emQuantities);
            if (emView == null)
            {
                throw new IntervalOutOfRangeException(intervalName);
            }
            var activeEnergy = emView.GetDifferenceInQuarterHours(ElectricityMeterQuantity.ActiveEnergy);
            var reactiveEnergyL = emView.GetDifferenceInQuarterHours(ElectricityMeterQuantity.ReactiveEnergyL);
            var reactiveEnergyC = emView.GetDifferenceInQuarterHours(ElectricityMeterQuantity.ReactiveEnergyC);

            var ep = activeEnergy.Values().ToArray();
            var eqL = reactiveEnergyL.Values().ToArray();
            var eqC = reactiveEnergyC.Values().ToArray();
            var cosFi = new float[ep.Length];

            for (int i = 0; i < ep.Length; i++)
            {
                cosFi[i] = ElectricityUtil.CalcCosFi(ep[i], eqL[i] - eqC[i]);
            }

            var distribution = CalcDistribution(cosFi);

            return DistributionToItems(distribution);
        }

        public PowerFactorDistributionItem[] DistributionToItems(Dictionary<string, int> distribution)
        {
            var items = new List<PowerFactorDistributionItem>();

            foreach (KeyValuePair<string, int> entry in distribution)
            {
                items.Add(new PowerFactorDistributionItem
                {
                    Kind = null,
                    Phase = 0,
                    Range = entry.Key,
                    Value = entry.Value,
                });
            }

            return items.ToArray();
        }

        public Dictionary<string, int> CalcDistribution(float[] cosFi)
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