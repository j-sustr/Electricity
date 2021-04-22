using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.IntegrationTests
{
    public class FakeSession : ISession
    {
        public Dictionary<string, byte[]> Store { get; set; } = new Dictionary<string, byte[]>();

        public bool IsAvailable => throw new System.NotImplementedException();

        public string Id => throw new System.NotImplementedException();

        public IEnumerable<string> Keys => throw new System.NotImplementedException();

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new System.NotImplementedException();
        }

        public void Set(string key, byte[] value)
        {
            Store.Add(key, value);
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            return Store.TryGetValue(key, out value);
        }
    }
}