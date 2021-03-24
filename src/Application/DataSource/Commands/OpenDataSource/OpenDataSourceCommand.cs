using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.DataSource.Commands.OpenDataSource
{
    public class OpenDataSourceCommand : IRequest
    {
    }

    public class OpenDataSourceCommandHandler : IRequestHandler<OpenDataSourceCommand>
    {
        public OpenDataSourceCommandHandler()
        {
        }

        public Task<Unit> Handle(OpenDataSourceCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}