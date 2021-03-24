using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using KMB.DataSource.DB;
using KMB.DataSource.File;

namespace Electricity.Infrastructure.DataSource
{
    public class DataSourceFactory : IDataSourceFactory
    {
        public KMB.DataSource.DataSource CreateDataSource(DataSourceConfig config)
        {
            KMB.DataSource.DataSource ds = null;
            if (config.DataSourceType == DataSourceType.DB)
            {
                var db = config.DBConnectionParams;
                ds = new DBDataSource(db.Server, db.DBName, db.Username, db.Password);
            }
            else if (config.DataSourceType == DataSourceType.File)
            {
                ds = new FileDataSource(config.CEAFileName);
            }
            return ds;
        }
    }
}