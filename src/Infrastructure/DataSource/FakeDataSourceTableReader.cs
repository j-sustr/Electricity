using System;
using System.Collections.Generic;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Interfaces.Queries;

namespace Electricity.Infrastructure.DataSource
{
    public class FakeDataSourceTableReader : ITable
    {
        public FakeDataSourceTableReader()
        {

        }

        public IEnumerable<Tuple<DateTime, float[]>> GetRows(IGetRowsQuery query)
        {
            throw new NotImplementedException();
        }
    }
}