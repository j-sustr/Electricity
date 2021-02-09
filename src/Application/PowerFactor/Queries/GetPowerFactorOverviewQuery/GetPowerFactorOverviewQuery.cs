using DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
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
        public Interval[] Intervals { get; set; }

        public Guid[] GroupIds { get; set; }
    }

    public class GetPowerFactorOverviewQueryHandler : IRequestHandler<GetPowerFactorOverviewQuery, PowerFactorOverviewDto>
    {
        private ICurrentUserService _currentUserService;
        private IGroupService _groupService;
        private ITableCollection _tableCollection;

        public GetPowerFactorOverviewQueryHandler(
            ICurrentUserService currentUserService,
            IGroupService groupService,
            ITableCollection tableCollection)
        {
            _currentUserService = currentUserService;
            _groupService = groupService;
            _tableCollection = tableCollection;
        }

        public Task<PowerFactorOverviewDto> Handle(GetPowerFactorOverviewQuery request, CancellationToken cancellationToken)
        {
            string userId = _currentUserService.UserId;
            if (userId == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (request.Intervals == null)
            {
                throw new ArgumentException("intervals is null");
            }

            var dto = new PowerFactorOverviewDto();

            var userGroups = _groupService.GetUserGroups(userId);

            var dataList = new List<PowerFactorOverviewIntervalData>();

            foreach (var interval in request.Intervals)
            {
                var data = GetDataForInterval(userGroups, interval);

                dataList.Add(data);
            }

            dto.Data = dataList;
            return Task.FromResult(dto);
        }

        public PowerFactorOverviewIntervalData GetDataForInterval(Group[] userGroups, Interval interval)
        {
            if (userGroups == null)
            {
                return null;
            }

            var items = userGroups.Select(g =>
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

                return new PowerFactorOverviewItem
                {
                    DeviceName = g.Name,
                    ActiveEnergy = activeEnergy,
                    ReactiveEnergyL = reactiveEnergyL,
                    ReactiveEnergyC = reactiveEnergyC,
                    TanFi = tanFi,
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