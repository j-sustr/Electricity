using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using DS = DataSource;

namespace Electricity.Infrastructure.DataSource
{

    public class DataSourceManager : IDataSourceManager
    {
        private readonly ConcurrentDictionary<Guid, DS.DataSource> dataSources = new ConcurrentDictionary<Guid, DS.DataSource>();
        private readonly ConcurrentDictionary<Guid, DataSourceConfig> dataSourceConfigs = new ConcurrentDictionary<Guid, DataSourceConfig>();

        public DataSourceManager()
        {

        }

        public Guid CreateDataSource(DataSourceConfig config)
        {
            DS.DataSource newDataSource = InstantiateDataSource(config);

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

        public DS.DataSource GetDataSource(Guid id)
        {
            DS.DataSource ds;
            if (dataSources.TryGetValue(id, out ds))
            {
                return ds;
            }
            DataSourceConfig config = null;
            if (dataSourceConfigs.TryGetValue(id, out config))
            {
                ds = InstantiateDataSource(config);
                dataSources.AddOrUpdate(id, (_) => ds, (_, __) => ds);
                return ds;
            }

            return null;
        }

        private DS.DataSource InstantiateDataSource(DataSourceConfig config)
        {
            DS.DataSource ds = null;
            if (config.DataSourceType == DataSourceType.DB)
            {
                var db = config.DBConnectionParams;
                ds = new DS.DBDataSource(db.Server, db.DBName, db.Username, db.Password);
            }
            else if (config.DataSourceType == DataSourceType.File)
            {
                ds = new DS.FileDataSource(config.CEAFileName);
            }
            return ds;
        }
    }
}