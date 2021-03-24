using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Extensions;


namespace Electricity.Infrastructure.DataSource
{
    public class DataSourceManager : IDataSourceManager
    {
        private readonly IDataSourceFactory _dsFactory;
        private readonly ConcurrentDictionary<Guid, KMB.DataSource.DataSource> dataSources = new ConcurrentDictionary<Guid, KMB.DataSource.DataSource>();
        private readonly ConcurrentDictionary<Guid, DataSourceConfig> dataSourceConfigs = new ConcurrentDictionary<Guid, DataSourceConfig>();

        public DataSourceManager(IDataSourceFactory dsFactory)
        {
            _dsFactory = dsFactory;
        }

        public Guid CreateDataSource(DataSourceConfig config)
        {
            KMB.DataSource.DataSource newDataSource = _dsFactory.CreateDataSource(config);

            var id = Guid.NewGuid();
            var configCopy = config.DeepClone();
            dataSourceConfigs.TryAdd(id, configCopy);
            dataSources.TryAdd(id, newDataSource);
            return id;
        }

        public bool DeleteDataSource(Guid id)
        {
            return dataSources.TryRemove(id, out var _);
        }

        public KMB.DataSource.DataSource GetDataSource(Guid id)
        {
            KMB.DataSource.DataSource ds;
            if (dataSources.TryGetValue(id, out ds))
            {
                return ds;
            }
            DataSourceConfig config = null;
            if (dataSourceConfigs.TryGetValue(id, out config))
            {
                ds = _dsFactory.CreateDataSource(config);
                dataSources.AddOrUpdate(id, (_) => ds, (_, __) => ds);
                return ds;
            }

            return null;
        }
    }
}