using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
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

                return new PowerFactorOverviewItemDto
                {
                    Interval = 0,
                    ActiveEnergy = 0,
                    ReactiveEnergyL = 0,
                    ReactiveEnergyC = 0,
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