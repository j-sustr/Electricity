using System;
using System.Collections.Generic;
using Electricity.Application.Common.Interfaces.Queries;

namespace Electricity.Application.Common.Interfaces
{
    public interface ITable
    {
        public IEnumerable<Tuple<DateTime, float[]>> GetRows(IGetRowsQuery query);
    }
}