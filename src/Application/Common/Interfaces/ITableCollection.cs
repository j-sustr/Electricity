using System;

namespace Electricity.Application.Common.Interfaces
{
    public interface ITableCollection
    {
        public ITable GetTable(Guid groupId, byte arch);
    }
}