using System;
using System.Threading;
using System.Threading.Tasks;
using Electricity.Application.Common.Interfaces;
using MediatR;

namespace Electricity.Application.Groups.Queries.GetUserGroups
{
    public class GetUserGroupsQuery : IRequest<UserGroupsDto>
    {
        public Guid UserId { get; set; }
    }

    public class GetUserGroupsQueryHandler : IRequestHandler<GetUserGroupsQuery, UserGroupsDto>
    {

        private readonly IGroupService _service;
        public GetUserGroupsQueryHandler(IGroupService service)
        {
            _service = service;
        }

        public async Task<UserGroupsDto> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new UserGroupsDto
            {
                Groups = _service.GetUserGroups(request.UserId)
            });
        }
    }
}