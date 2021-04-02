using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Extensions;
using Electricity.Application.Common.Exceptions;
using Electricity.Infrastructure.DataSource.Abstractions;

namespace Electricity.Infrastructure.DataSource
{
    public class DataSourceManager : IDataSourceManager
    {
        private readonly ITenantProvider _tenantProvider;
        private readonly IDataSourceFactory _dsFactory;
        private readonly DataSourceCache _dsCache;

        public DataSourceManager(
            ITenantProvider tenantProvider,
            IDataSourceFactory dsFactory,
            DataSourceCache dsCache
            )
        {
            _tenantProvider = tenantProvider;
            _dsFactory = dsFactory;
            _dsCache = dsCache;
        }

        public (Guid, KMB.DataSource.DataSource) CreateDataSource(DataSourceCreationParams creationParams)
        {
            KMB.DataSource.DataSource newDataSource = _dsFactory.CreateDataSource(creationParams);

            var id = Guid.NewGuid();
            _dsCache.Add(id, newDataSource);
            return (id, newDataSource);
        }

        public KMB.DataSource.DataSource GetDataSource()
        {
            var tenant = GetTenant();
            var id = tenant.DataSourceId;

            KMB.DataSource.DataSource ds;
            if (_dsCache.TryGetDataSource(id, out ds))
            {
                return ds;
            }

            var creationParams = tenant.DataSourceCreationParams;
            var (newId, newDS) = CreateDataSource(creationParams);
            tenant.DataSourceId = newId;
            return newDS;
        }

        public bool DeleteDataSource()
        {
            var tenant = GetTenant();
            return _dsCache.TryRemove(tenant.DataSourceId);
        }

        private Tenant GetTenant()
        {
            var tenant = _tenantProvider.GetTenant();
            if (tenant == null)
            {
                throw new UnknownTenantException();
            }
            return tenant;
        }
    }
}