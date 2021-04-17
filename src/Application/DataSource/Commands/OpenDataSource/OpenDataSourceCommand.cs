using Electricity.Application.Common.Abstractions;
using Electricity.Application.Common.Enums;
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
    public class OpenDataSourceCommand : IRequest<DataSourceInfoDto>
    {
        public TenantDto Tenant { get; set; }
    }

    public class OpenDataSourceCommandHandler : IRequestHandler<OpenDataSourceCommand, DataSourceInfoDto>
    {
        private readonly ITenantProvider _tenantProvider;

        private readonly IDataSourceManager _dsManager;

        private readonly ITenantService _tenantService;

        private readonly IMultiTenantStore<Tenant> _tenantStore;

        public OpenDataSourceCommandHandler(
            ITenantProvider tenantProvider,
            IDataSourceManager dsManager,
            ITenantService tenantService,
            IMultiTenantStore<Tenant> tenantStore
            )
        {
            _tenantProvider = tenantProvider;
            _dsManager = dsManager;
            _tenantService = tenantService;
            _tenantStore = tenantStore;
        }

        public async Task<DataSourceInfoDto> Handle(OpenDataSourceCommand request, CancellationToken cancellationToken)
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

            var existingTenant = _tenantProvider.GetTenant();
            if (existingTenant != null)
            {
                TryDeleteDataSource();
                tenant.Id = existingTenant.Id;
                tenant.Identifier = existingTenant.Identifier;
                await UpdateTenant(tenant);
            } 
            else
            {
                await AddTenant(tenant);
            }


            IDisposable connection = null;
            try
            {
                var (dsId, ds) = _dsManager.CreateDataSource(tenant.DataSourceCreationParams);
                connection = ds.NewConnection();
                tenant.DataSourceId = dsId;
                return null;
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


            string name;
            if (request.Tenant.DataSourceType == DataSourceType.DB)
            {
                name = request.Tenant.DBConnectionParams.DBName;
            }
            else
            {
                name = request.Tenant.CEAFileName;
            }

            return new DataSourceInfoDto
            {
                Name = name
            };
        }

        public async Task UpdateTenant(Tenant tenant)
        {
            var result = await _tenantStore.TryUpdateAsync(tenant);
            if (!result)
                throw new Exception("update tenant failed");
        }

        public async Task AddTenant(Tenant tenant)
        {
            var result = _tenantService.SetTenantIdentifier(tenant.Identifier);
            if (!result)
                throw new Exception("set tenant identifier failed");

            result = await _tenantStore.TryAddAsync(tenant);
            if (!result)
                throw new Exception("add tenant failed");
        }

        public void TryDeleteDataSource()
        {
            try
            {
                _dsManager.DeleteDataSource();
            }
            catch (UnknownTenantException ex) {}
        }
    }
}