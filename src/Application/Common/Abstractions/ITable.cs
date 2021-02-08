using System;
using System.Collections.Generic;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Queries;

namespace Electricity.Application.Common.Interfaces
{
    public interface ITable
    {
        public Interval GetInterval();
        public IEnumerable<Tuple<DateTime, float[]>> GetRows(GetRowsQuery query);
    }
}