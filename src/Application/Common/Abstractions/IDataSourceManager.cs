using System;
using Electricity.Application.Common.Models;

namespace Electricity.Application.Common.Interfaces
{

    public interface IDataSourceManager
    {
        Guid CreateDataSource(DataSourceConfig config);
        DataSource.DataSource GetDataSource(Guid id);
        bool DeleteDataSource(Guid id);
    }
}