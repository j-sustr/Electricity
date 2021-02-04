using System;
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
        ICurrentUserService _currentUserService;
        IGroupService _groupService;
        ITableCollection _tableCollection;

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

            var data = userGroups.Select(g =>
            {
                var emTable = _tableCollection.GetTable(g.ID, (byte)Arch.ElectricityMeter);

                var rows = emTable.GetRows(new GetRowsQuery
                {
                    Range = request.Interval1,
                    Quantities = new Quantity[] {
                        new Quantity("3EP", "Wh"),
                        new Quantity("3EQL", "varh"),
                        new Quantity("3EQC", "varh"),
                    }
                });

                var firstRow = rows.First();
                var lastRow = rows.Last();

                var activeEnergy = lastRow.Item2[0] - firstRow.Item2[0];
                var reactiveEnergyL = lastRow.Item2[1] - firstRow.Item2[1];
                var reactiveEnergyC = lastRow.Item2[2] - firstRow.Item2[2];

                return new PowerFactorOverviewItemDto
                {
                    Interval = 0,
                    ActiveEnergy = activeEnergy,
                    ReactiveEnergyL = reactiveEnergyL,
                    ReactiveEnergyC = reactiveEnergyC,
                    CosFi = 0
                };
            }).ToList();

            var result = new PowerFactorOverviewDto
            {
                Items = data
            };
            return Task.FromResult(result);
        }
    }
}