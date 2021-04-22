using Electricity.Application.Common.Models;
using System;

namespace Electricity.Application.Common.Interfaces
{
    public interface IArchiveRepository
    {
        public IArchive GetArchive(Guid groupId, byte arch);

    }
}