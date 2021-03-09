using DataSource;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Utils;
using System;
using System.Collections.Generic;

using DS = DataSource;

namespace Electricity.Infrastructure.DataSource.Fake
{
    public class FakeDataSourceFactory : IDataSourceFactory
    {
        private int _seed;

        public List<Group> UserGroups = new List<Group>{
            new Group(GuidUtil.IntToGuid(1), "group-1"),
            new Group(GuidUtil.IntToGuid(2), "group-2"),
            new Group(GuidUtil.IntToGuid(3), "group-3"),
            new Group(GuidUtil.IntToGuid(4), "group-4"),
            new Group(GuidUtil.IntToGuid(5), "group-5"),
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
            ds.Groups = UserGroups;
            return ds;
        }
    }
}