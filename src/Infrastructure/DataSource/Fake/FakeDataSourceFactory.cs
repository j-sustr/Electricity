using DataSource;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using System;
using System.Collections.Generic;

using DS = DataSource;

namespace Electricity.Infrastructure.DataSource.Fake
{
    public class FakeDataSourceFactory : IDataSourceFactory
    {
        private int _seed;

        public List<Group> Groups = new List<Group>{
            new Group(Guid.NewGuid(), "group-1"),
            new Group(Guid.NewGuid(), "group-2"),
            new Group(Guid.NewGuid(), "group-3"),
        };

        private BoundedInterval _interval { get; set; }

        public FakeDataSourceFactory(int seed, BoundedInterval interval)
        {
            _seed = seed;
            _interval = interval;
        }

        public DS.DataSource CreateDataSource(DataSourceConfig config)
        {
            var ds = new FakeDataSource(_seed, _interval);
            ds.Groups = Groups;
            return ds;
        }
    }
}