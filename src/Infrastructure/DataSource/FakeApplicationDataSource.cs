using System;
using System.Collections.Generic;
using DataSource;
using Electricity.Application.Common.Interfaces;

namespace Electricity.Infrastructure.DataSource
{
    public class FakeApplicationDataSource : ITableCollection
    {
        int _seed;

        public FakeApplicationDataSource(int seed)
        {
            _seed = seed;
        }

        public ITable GetTable(Guid groupId, byte arch)
        {
            return new FakeDataSourceTableReader(_seed);
        }
    }
}