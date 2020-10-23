using System;
using System.Collections.Generic;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using DS = DataSource;

namespace Electricity.Infrastructure.DataSource
{

    public class DataSourceManager : IDataSourceManager
    {
        Dictionary<Guid, DS.DataSource> dataSources = new Dictionary<Guid, DS.DataSource>();
        Dictionary<Guid, DataSourceConfig> dataSourceConfigs = new Dictionary<Guid, DataSourceConfig>();

        public DataSourceManager()
        {

        }

        public Guid CreateDataSource(DataSourceConfig config)
        {
            DS.DataSource newDataSource = InstantiateDataSource(config);

            var id = Guid.NewGuid();
            var configCopy = config.DeepClone();
            dataSourceConfigs.Add(id, configCopy);
            dataSources.Add(id, newDataSource);
            return id;
        }

        public bool DeleteDataSource(Guid id)
        {
            return dataSources.Remove(id);
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
                dataSources.Add(id, ds);
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