using AutoMapper;
using AutoMapper.QueryableExtensions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models.Dtos;
using KMB.DataSource;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.Groups.Queries.GetUserRecordGroupInfos
{
    public class GetUserRecordGroupInfosQuery: IRequest<GroupInfoDto[]>
    {
    }

    public class GetUserRecordGroupInfosQueryHandler : IRequestHandler<GetUserRecordGroupInfosQuery, GroupInfoDto[]>
    {
        private readonly IGroupRepository _service;
        private readonly IMapper _mapper;

        public GetUserRecordGroupInfosQueryHandler(
            IGroupRepository service,
            IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<GroupInfoDto[]> Handle(GetUserRecordGroupInfosQuery request, CancellationToken cancellationToken)
        {
            var infos = _service.GetUserRecordGroupInfos();

            var infosDto = Queryable
                    .AsQueryable(infos)
                    .ProjectTo<GroupInfoDto>(_mapper.ConfigurationProvider)
                    .ToArray();

            return await Task.FromResult(infosDto);
        }
    }
}


