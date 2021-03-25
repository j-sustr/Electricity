using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Dtos;
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
        public TenantDto Tenant { get; set; }
    }

    public class OpenDataSourceCommandHandler : IRequestHandler<OpenDataSourceCommand, bool>
    {
        private readonly IMultiTenantStore<Tenant> _tenantStore;

        private readonly IDataSourceManager _dsManager;

        public OpenDataSourceCommandHandler(
            IMultiTenantStore<Tenant> tenantStore,
            IDataSourceManager dsManager)
        {
            _tenantStore = tenantStore;
            _dsManager = dsManager;
        }

        public async Task<bool> Handle(OpenDataSourceCommand request, CancellationToken cancellationToken)
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = Guid.NewGuid().ToString(),
                Name = "(empty)",
                DataSourceType = request.Tenant.DataSourceType,
                ConnectionString = "(empty)",
                CEAFileName = request.Tenant.CEAFileName,
                DBConnectionParams = request.Tenant.DBConnectionParams,
                DataSourceId = null,
            };

            tenant.DataSourceId = _dsManager.CreateDataSource(tenant.DataSourceConfig);

            if (tenant.DataSourceId == null)
            {
                throw new UnknownTenantException();
            }

            return await _tenantStore.TryAddAsync(tenant);
        }
    }
}