using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Abstractions
{
    public interface IDataSourceCache
    {
        public bool Add(Guid id, KMB.DataSource.DataSource ds);

        public bool TryGetDataSource(Guid id, out KMB.DataSource.DataSource ds);

        public bool TryRemove(Guid id);

        public void Clear();
    }
}