using Electricity.Application.Common.Models;
using System;

namespace Electricity.Application.Common.Interfaces
{
    public interface IArchiveRepository
    {
        public ITable GetTable(Guid groupId, byte arch);

        public Interval GetInterval(Guid? groupId, byte arch);
    }
}