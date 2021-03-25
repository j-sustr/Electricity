using Electricity.Application.Common.Models;
using Finbuckle.MultiTenant;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.DataSource.Commands.OpenDataSource
{
    public class OpenDataSourceCommand : IRequest<bool>
    {
        public Tenant Tenant { get; set; }
    }

    public class OpenDataSourceCommandHandler : IRequestHandler<OpenDataSourceCommand, bool>
    {
        private readonly IMultiTenantStore<Tenant> _tenantStore;

        public OpenDataSourceCommandHandler(IMultiTenantStore<Tenant> tenantStore)
        {
            _tenantStore = tenantStore;
        }

        public async Task<bool> Handle(OpenDataSourceCommand request, CancellationToken cancellationToken)
        {
            return await _tenantStore.TryAddAsync(request.Tenant);
        }
    }
}