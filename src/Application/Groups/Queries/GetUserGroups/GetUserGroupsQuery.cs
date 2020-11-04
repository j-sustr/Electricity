using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models.Dtos;
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
        private readonly IMapper _mapper;


        public GetUserGroupsQueryHandler(IGroupService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<UserGroupsDto> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
        {
            var groups = _service.GetUserGroups(request.UserId);

            return await Task.FromResult(new UserGroupsDto
            {
                Groups = Queryable
                    .AsQueryable(groups)
                    .ProjectTo<GroupDto>(_mapper.ConfigurationProvider)
                    .ToList()
            });
        }
    }
}