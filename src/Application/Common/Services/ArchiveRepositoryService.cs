using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public class ArchiveRepositoryService
    {
        protected readonly IArchiveRepository _archiveRepository;
        protected readonly IGroupRepository _groupRepository;

        public ArchiveRepositoryService(
            IArchiveRepository archiveRepository,
            IGroupRepository groupRepository)
        {
            _archiveRepository = archiveRepository;
            _groupRepository = groupRepository;
        }

        public bool HasInterval(Interval interval, byte arch)
        {
            return GetIntervalOverlap(interval, arch).Equals(interval);
        }

        public Interval GetIntervalOverlap(Interval interval, byte arch)
        {
            var infos = _groupRepository.GetUserRecordGroupInfos();

            var overlap = Interval.Unbounded;

            foreach (var info in infos)
            {
                var archInfo = info.GetArchiveInfo(arch);

                overlap = Interval.FromDateRange(archInfo.Range).GetOverlap(overlap);
            }

            return overlap;
        }
    }
}
