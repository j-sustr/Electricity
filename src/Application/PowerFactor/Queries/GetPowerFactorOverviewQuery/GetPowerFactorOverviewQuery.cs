using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models.Queries;
using MediatR;

namespace Electricity.Application.PowerFactor.Queries.GetPowerFactorOverviewQuery
{
    public class GetPowerFactorOverviewQuery : IRequest<PowerFactorOverviewDto>
    {
        public Tuple<DateTime, DateTime> Interval1 { get; set; }
        public Tuple<DateTime, DateTime> Interval2 { get; set; }

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

            var userGroups = _groupService.GetUserGroups(userId);

            var data1 = GetDataForInterval(userGroups, request.Interval1);
            var data2 = GetDataForInterval(userGroups, request.Interval2);

            var result = new PowerFactorOverviewDto
            {
                Interval1Items = data1?.ToList(),
                Interval2Items = data2?.ToList(),
            };
            return Task.FromResult(result);
        }

        public IEnumerable<PowerFactorOverviewItemDto> GetDataForInterval(Group[] userGroups, Tuple<DateTime, DateTime> interval)
        {
            if (interval == null)
            {
                return null;
            }

            return userGroups.Select(g =>
            {
                var emTable = _tableCollection.GetTable(g.ID, (byte)Arch.ElectricityMeter);
                var quantities = new Quantity[] {
                    new Quantity("3EP", "Wh"),
                    new Quantity("3EQL", "varh"),
                    new Quantity("3EQC", "varh"),
                };

                var rows1 = emTable.GetRows(new GetRowsQuery
                {
                    Range = interval,
                    Quantities = quantities,
                });

                if (rows1.Count() == 0)
                {
                    return null;
                }

                var firstRow = rows1.First();
                var lastRow = rows1.Last();

                var activeEnergy = lastRow.Item2[0] - firstRow.Item2[0];
                var reactiveEnergyL = lastRow.Item2[1] - firstRow.Item2[1];
                var reactiveEnergyC = lastRow.Item2[2] - firstRow.Item2[2];

                var tanFi = reactiveEnergyL / activeEnergy;

                return new PowerFactorOverviewItemDto
                {
                    DeviceName = g.Name,
                    Interval = 0,
                    ActiveEnergy = activeEnergy,
                    ReactiveEnergyL = reactiveEnergyL,
                    ReactiveEnergyC = reactiveEnergyC,
                    TanFi = tanFi
                };
            });
        }
    }
}