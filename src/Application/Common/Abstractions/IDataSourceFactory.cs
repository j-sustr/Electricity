using Electricity.Application.Common.Models;

namespace Electricity.Application.Common.Interfaces
{
    public interface IDataSourceFactory
    {
        KMB.DataSource.DataSource CreateDataSource(DataSourceConfig config);
    }
}