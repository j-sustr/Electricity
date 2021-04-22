using KMB.DataSource;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Utils;
using System.Collections.Generic;
using Electricity.Infrastructure.DataSource.Abstractions;
using Electricity.Infrastructure.DataSource.Util;

namespace Electricity.Infrastructure.DataSource.Fake
{
    public class FakeDataSourceFactory : IDataSourceFactory
    {
        private int _seed;

        public FakeUserData[] Users = FakeData.GetUsers();

        public FakeDataSourceFactory(int seed)
        {
            _seed = seed;
        }

        public KMB.DataSource.DataSource CreateDataSource(DataSourceCreationParams creationParams)
        {
            var ds = new FakeDataSource(_seed);
            ds.Users = Users;
            return ds;
        }
    }
}