using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using DS = DataSource;

namespace Electricity.Infrastructure.DataSource.Fake
{
    public class FakeDataSourceFactory : IDataSourceFactory
    {
        private int _seed;

        private BoundedInterval _interval { get; set; }

        public FakeDataSourceFactory(int seed, BoundedInterval interval)
        {
            _seed = seed;
            _interval = interval;
        }

        public DS.DataSource CreateDataSource(DataSourceConfig config)
        {
            return new FakeDataSource(_seed, _interval);
        }
    }
}