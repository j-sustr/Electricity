using System;

namespace Electricity.Application.Common.Interfaces
{

    public interface IDataSourceProvider
    {
        DataSource.DataSource GetDataSource(Guid id);
    }
}
