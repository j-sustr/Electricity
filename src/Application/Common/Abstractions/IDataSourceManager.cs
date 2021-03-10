using System;
using Electricity.Application.Common.Models;
using DS = global::DataSource;

namespace Electricity.Application.Common.Interfaces
{
    using DataSource;

    public interface IDataSourceManager
    {
        Guid CreateDataSource(DataSourceConfig config);

        DS.DataSource GetDataSource(Guid id);

        bool DeleteDataSource(Guid id);
    }
}