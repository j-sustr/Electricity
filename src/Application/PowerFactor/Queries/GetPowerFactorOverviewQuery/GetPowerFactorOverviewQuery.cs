using AutoMapper;
using DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Common.Models.Queries;
using MediatR;
using Newtonsoft.Json;
using NJsonSchema.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.PowerFactor.Queries.GetPowerFactorOverviewQuery
{
    public class GetPowerFactorOverviewQuery : IRequest<PowerFactorOverviewDto>
    {
        public IntervalDto Interval1 { get; set; }

        public IntervalDto? Interval2 { get; set; }

        public Guid[] GroupIds { get; set; }
    }

    public class GetPowerFactorOverviewQueryHandler : IRequestHandler<GetPowerFactorOverviewQuery, PowerFactorOverviewDto>
    {
        private readonly IGroupService _groupService;
        private readonly ITableCollection _tableCollection;
        private readonly IMapper _mapper;

        public GetPowerFactorOverviewQueryHandler(
            IGroupService groupService,
            ITableCollection tableCollection,
            IMapper mapper)
        {
            _groupService = groupService;
            _tableCollection = tableCollection;
            _mapper = mapper;
        }

        public Task<PowerFactorOverviewDto> Handle(GetPowerFactorOverviewQuery request, CancellationToken cancellationToken)
        {
            var interval1 = _mapper.Map<Interval>(request.Interval1);
            var interval2 = _mapper.Map<Interval>(request.Interval2);

            var dto = new PowerFactorOverviewDto();

            var userGroups = _groupService.GetUserGroups();

            var dataList = new List<PowerFactorOverviewIntervalData>();

            var data = GetDataForInterval(userGroups, interval1);
            dataList.Add(data);

            if (request.Interval2 != null)
            {
                data = GetDataForInterval(userGroups, interval1);
                dataList.Add(data);
            }

            dto.Data = dataList;
            return Task.FromResult(dto);
        }

        public PowerFactorOverviewIntervalData GetDataForInterval(Group[] groups, Interval interval)
        {
            if (groups == null)
            {
                return null;
            }

            var items = groups.Select(g =>
            {
                var emTable = _tableCollection.GetTable(g.ID, (byte)Arch.ElectricityMeter);
                var quantities = new Quantity[] {
                    new Quantity("3EP", "Wh"),
                    new Quantity("3EQL", "varh"),
                    new Quantity("3EQC", "varh"),
                };

                var rows = emTable.GetRows(new GetRowsQuery
                {
                    Interval = interval,
                    Quantities = quantities,
                });

                if (rows.Count() == 0)
                {
                    return null;
                }

                var rowsInterval = GetRowsInterval(rows);

                var firstRow = rows.First();
                var lastRow = rows.Last();

                var activeEnergy = lastRow.Item2[0] - firstRow.Item2[0];
                var reactiveEnergyL = lastRow.Item2[1] - firstRow.Item2[1];
                var reactiveEnergyC = lastRow.Item2[2] - firstRow.Item2[2];

                var tanFi = reactiveEnergyL / activeEnergy;

                var cosFi = (float)Math.Cos(Math.Atan(tanFi));

                return new PowerFactorOverviewItem
                {
                    DeviceName = g.Name,
                    ActiveEnergy = activeEnergy,
                    ReactiveEnergyL = reactiveEnergyL,
                    ReactiveEnergyC = reactiveEnergyC,
                    CosFi = cosFi,
                    Interval = rowsInterval
                };
            });

            return new PowerFactorOverviewIntervalData
            {
                Interval = null,
                Items = items.ToList()
            };
        }

        public Interval GetRowsInterval(IEnumerable<Tuple<DateTime, float[]>> rows)
        {
            var start = rows.First().Item1;
            var end = rows.Last().Item1;
            return new Interval(start, end);
        }
    }
}