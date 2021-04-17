using Electricity.Application.Common.Abstractions;
using Electricity.Infrastructure.DataSource.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Infrastructure.DataSource
{
    internal class DataSourceCacheItem
    {
        public KMB.DataSource.DataSource dataSource;
        public TimeSpan ttl;
        // public HashSet<string> users; 
    }

    public class DataSourceCache : IDataSourceCache
    {
        private readonly TimeSpan TTL = TimeSpan.FromMinutes(30);

        private readonly ConcurrentDictionary<Guid, DataSourceCacheItem> _items = new ConcurrentDictionary<Guid, DataSourceCacheItem>();

        private DateTime _lastTtlUpdateTime = DateTime.Now;
        private CancellationTokenSource _clearTaskSource = null;

        public bool Add(Guid id, KMB.DataSource.DataSource ds)
        {
            TryRemove(id);

            var item = new DataSourceCacheItem
            {
                dataSource = ds,
                ttl = TTL
            };
            _items.AddOrUpdate(id, (_) => item, (_, __) => item);
            return true;
        }

        public bool TryGetDataSource(Guid id, out KMB.DataSource.DataSource ds)
        {
            // Update(id);

            var success = _items.TryGetValue(id, out var item);
            ds = item?.dataSource;
            return success;
        }

        public bool TryRemove(Guid id)
        {
            // Update(id);

            var success = _items.TryRemove(id, out var item);
            item?.dataSource.Dispose();
            return success;
        }

        private void Update(Guid id)
        {
            TryResetTtl(id);
            UpdateTtlAndClearDead(id);
            ResetClearTask();
        }

        public void TryResetTtl(Guid id)
        {
            if (_items.TryGetValue(id, out var item))
            {
                item.ttl = TTL;
            }
        }

        public void UpdateTtlAndClearDead(Guid? ignoreId)
        {
            var now = DateTime.Now;
            var deltaTime =  now - _lastTtlUpdateTime;
            _lastTtlUpdateTime = now;

            foreach (var id in _items.Keys)
            {
                if (id == ignoreId)
                    continue;

                if (_items.TryGetValue(id, out var item))
                {
                    item.ttl -= deltaTime;
                    if (item.ttl <= TimeSpan.Zero)
                    {
                        item.dataSource.Dispose();
                        _items.TryRemove(id, out var _);
                    }
                }
            }
        }

        public void ResetClearTask()
        {
            if (_clearTaskSource != null)
            {
                _clearTaskSource.Cancel();
            }

            _clearTaskSource = new CancellationTokenSource();
            Task.Delay(TTL, _clearTaskSource.Token).ContinueWith(_ =>
            {
                UpdateTtlAndClearDead(null);
            });
        }

        public void Clear()
        {
            foreach (var item in _items.Values)
            {
                item.dataSource.Dispose();
            }
            _items.Clear();
        }
    }
}