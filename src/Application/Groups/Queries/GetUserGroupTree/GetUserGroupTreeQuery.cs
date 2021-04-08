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
        private readonly IGroupRepository _service;
        private readonly IMapper _mapper;

        public GetUserGroupTreeQueryHandler(
            IGroupRepository service,
            IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<GroupTreeNodeDto> Handle(GetUserGroupTreeQuery request, CancellationToken cancellationToken)
        {
            var tree = _service.GetUserGroupTree();

            var treeDto = _mapper.Map<GroupTreeNodeDto>(tree);

            return await Task.FromResult(treeDto);
        }
    }
}