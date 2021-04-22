using Electricity.Application.Common.Models;

namespace Electricity.Infrastructure.DataSource.Abstractions
{
    public interface IDataSourceFactory
    {
        KMB.DataSource.DataSource CreateDataSource(DataSourceCreationParams config);
    }
}