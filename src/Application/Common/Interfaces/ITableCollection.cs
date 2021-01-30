using System;
using System.Collections.Generic;
using DataSource;
using Electricity.Application.Common.Models;

namespace Electricity.Application.Common.Interfaces
{
    public interface ITableCollection
    {
        public ITable GetTable(Guid groupId, byte arch);
    }
}