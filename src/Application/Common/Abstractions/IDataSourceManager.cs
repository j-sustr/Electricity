using System;
using Electricity.Application.Common.Models;

namespace Electricity.Application.Common.Interfaces
{
    using KMB.DataSource;

    public interface IDataSourceManager
    {
        Guid CreateDataSource(DataSourceConfig config);

        KMB.DataSource.DataSource GetDataSource(Guid id);

        bool DeleteDataSource(Guid id);
    }
}