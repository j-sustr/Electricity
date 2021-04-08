using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.DataSource.Queries.GetDataSourceInfo
{
    public class GetDataSourceInfoQuery : IRequest<DataSourceInfoDto>
    {
        public Guid? GroupId { get; set; }
        public byte Arch { get; set; }
    }

    public class GetDataSourceInfoQueryHandler : IRequestHandler<GetDataSourceInfoQuery, DataSourceInfoDto>
    {
        private readonly IArchiveRepository _tableCollection;

        public GetDataSourceInfoQueryHandler(IArchiveRepository tableCollection)
        {
            _tableCollection = tableCollection;
        }

        public Task<DataSourceInfoDto> Handle(GetDataSourceInfoQuery request, CancellationToken cancellationToken)
        {
            var interval = _tableCollection.GetInterval(request.GroupId, request.Arch);
            if (interval == null)
            {
                throw new NotFoundException();
            }

            var dto = new DataSourceInfoDto
            {
                MinDatetime = interval?.Start,
                MaxDatetime = interval?.End
            };

            return Task.FromResult(dto);
        }
    }
}