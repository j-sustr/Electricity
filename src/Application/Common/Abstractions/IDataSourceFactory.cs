using Electricity.Application.Common.Models;
using DS = DataSource;

namespace Electricity.Application.Common.Interfaces
{
    public interface IDataSourceFactory
    {
        DS.DataSource CreateDataSource(DataSourceConfig config);
    }
}