using System;
using Electricity.Application.Common.Models;

namespace Electricity.Application.Common.Interfaces
{
    using KMB.DataSource;

    public interface IDataSourceManager
    {
        (Guid, KMB.DataSource.DataSource) CreateDataSource(DataSourceCreationParams creationParams);

        KMB.DataSource.DataSource GetDataSource();

        bool DeleteDataSource();
    }
}