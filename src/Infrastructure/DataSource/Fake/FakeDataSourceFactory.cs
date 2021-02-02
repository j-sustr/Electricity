using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using DS = DataSource;

namespace Electricity.Infrastructure.DataSource.Fake
{
    public class FakeDataSourceFactory : IDataSourceFactory
    {
        private int _seed;

        public FakeDataSourceFactory(int seed)
        {
            _seed = seed;
        }

        public DS.DataSource CreateDataSource(DataSourceConfig config)
        {
            return new FakeDataSource(_seed);
        }
    }
}