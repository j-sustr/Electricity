using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using DS = DataSource;

namespace Electricity.Infrastructure.DataSource.Fake
{
    public class FakeDataSourceFactory : IDataSourceFactory
    {
        private int _seed;

        private Interval _interval { get; set; }

        public FakeDataSourceFactory(int seed, Interval interval)
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