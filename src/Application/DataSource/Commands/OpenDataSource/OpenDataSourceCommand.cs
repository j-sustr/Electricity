using Electricity.Application.Common.Abstractions;
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
        private readonly IDataSourceManager _dsManager;

        private readonly ITenantService _tenantService;

        private readonly IMultiTenantStore<Tenant> _tenantStore;

        public OpenDataSourceCommandHandler(
            IDataSourceManager dsManager,
            ITenantService tenantService,
            IMultiTenantStore<Tenant> tenantStore
            )
        {
            _dsManager = dsManager;
            _tenantService = tenantService;
            _tenantStore = tenantStore;
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
            };

            var result = _tenantService.SetTenantIdentifier(tenant.Identifier);
            if (!result) return false;

            result = await _tenantStore.TryAddAsync(tenant);
            if (!result) return false;

            IDisposable connection = null;
            try
            {
                var (dsId, ds) = _dsManager.CreateDataSource(tenant.DataSourceCreationParams);
                connection = ds.NewConnection();
                tenant.DataSourceId = dsId;
                return false;
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection?.Dispose();
            }

            return false;
        }
    }
}