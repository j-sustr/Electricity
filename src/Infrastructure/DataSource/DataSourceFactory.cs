using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using DS = DataSource;

namespace Electricity.Infrastructure.DataSource
{
    public class DataSourceFactory : IDataSourceFactory
    {
        public DS.DataSource CreateDataSource(DataSourceConfig config)
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