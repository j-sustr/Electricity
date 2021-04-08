using AutoMapper;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.Groups.Queries.GetUserGroupInfoTree
{
    public class GetUserGroupInfoTreeQuery : IRequest<GroupInfoDto>
    {
    }

    public class GetUserGroupInfoTreeQueryHandler : IRequestHandler<GetUserGroupInfoTreeQuery, GroupInfoDto>
    {
        private readonly IGroupRepository _service;
        private readonly IMapper _mapper;

        public GetUserGroupInfoTreeQueryHandler(
            IGroupRepository service,
            IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<GroupInfoDto> Handle(GetUserGroupInfoTreeQuery request, CancellationToken cancellationToken)
        {
            var info = _service.GetUserGroupInfoTree();

            var dto = _mapper.Map<GroupInfoDto>(info);

            return await Task.FromResult(dto);
        }
    }
}
