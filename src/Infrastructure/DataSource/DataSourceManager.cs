using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Extensions;
using Electricity.Application.Common.Exceptions;

namespace Electricity.Infrastructure.DataSource
{
    public class DataSourceManager : IDataSourceManager
    {
        private readonly ITenantProvider _tenantProvider;
        private readonly IDataSourceFactory _dsFactory;
        // private readonly ICurrentUserService _currentUserService;

        private readonly ConcurrentDictionary<Guid, KMB.DataSource.DataSource> _dataSourceCache = new ConcurrentDictionary<Guid, KMB.DataSource.DataSource>();
        // private readonly ConcurrentDictionary<Guid, HashSet<string>> _dataSourceUsers = new ConcurrentDictionary<Guid, HashSet<string>>();

        public DataSourceManager(
            ITenantProvider tenantProvider,
            IDataSourceFactory dsFactory
            )
        {
            _tenantProvider = tenantProvider;
            _dsFactory = dsFactory;
        }

        public (Guid, KMB.DataSource.DataSource) CreateDataSource(DataSourceCreationParams creationParams)
        {
            KMB.DataSource.DataSource newDataSource = _dsFactory.CreateDataSource(creationParams);

            var id = Guid.NewGuid();
            _dataSourceCache.TryAdd(id, newDataSource);
            return (id, newDataSource);
        }

        public KMB.DataSource.DataSource GetDataSource()
        {
            var tenant = GetTenant();
            var id = tenant.DataSourceId;

            KMB.DataSource.DataSource ds;
            if (_dataSourceCache.TryGetValue(id, out ds))
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
            return _dataSourceCache.TryRemove(tenant.DataSourceId, out var _);
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