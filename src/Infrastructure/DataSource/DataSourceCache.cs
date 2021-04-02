using Electricity.Infrastructure.DataSource.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electricity.Infrastructure.DataSource
{
    public class DataSourceCache
    {
        private readonly ConcurrentDictionary<Guid, KMB.DataSource.DataSource> _cache = new ConcurrentDictionary<Guid, KMB.DataSource.DataSource>();
        // private readonly ConcurrentDictionary<Guid, HashSet<string>> _dataSourceUsers = new ConcurrentDictionary<Guid, HashSet<string>>();

        public bool Add(Guid id, KMB.DataSource.DataSource ds)
        {
            _cache.AddOrUpdate(id, (_) => ds, (_, __) => ds);
            return true;
        }

        public bool TryGetDataSource(Guid id, out KMB.DataSource.DataSource ds)
        {
            return _cache.TryGetValue(id, out ds);
        }

        public bool TryRemove(Guid id)
        {
            return _cache.TryRemove(id, out var _);
        }
    }
}