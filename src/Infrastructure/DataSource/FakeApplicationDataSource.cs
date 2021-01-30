using System;
using System.Collections.Generic;
using DataSource;
using Electricity.Application.Common.Interfaces;

namespace Electricity.Infrastructure.DataSource
{
    public class FakeApplicationDataSource : ITableCollection
    {
        public ITable GetTable(Guid groupId, byte arch)
        {
            return new FakeDataSourceTableReader();
        }
    }
}