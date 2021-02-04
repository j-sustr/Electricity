using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models.Dtos;
using MediatR;

namespace Electricity.Application.Groups.Queries.GetUserGroupTree
{
    public class GetUserGroupTreeQuery : IRequest<GroupTreeNodeDto>
    {

    }

    public class GetUserGroupTreeQueryHandler : IRequestHandler<GetUserGroupTreeQuery, GroupTreeNodeDto>
    {
        private readonly IGroupService _service;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;


        public GetUserGroupTreeQueryHandler(
            ICurrentUserService currentUserService,
            IGroupService service,
            IMapper mapper)
        {
            _currentUserService = currentUserService;
            _service = service;
            _mapper = mapper;
        }


        public async Task<GroupTreeNodeDto> Handle(GetUserGroupTreeQuery request, CancellationToken cancellationToken)
        {
            var tree = _service.GetUserGroupTree(_currentUserService.UserId);

            var treeDto = _mapper.Map<GroupTreeNodeDto>(tree);

            return await Task.FromResult(treeDto);
        }
    }
}