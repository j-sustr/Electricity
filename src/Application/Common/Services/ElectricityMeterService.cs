using KMB.DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Electricity.Application.Common.Models.Queries;

namespace Electricity.Application.Common.Services
{
    public class ElectricityMeterService : ArchiveRepositoryService
    {
        public ElectricityMeterService(
            IArchiveRepository archiveRepository,
            IGroupRepository groupRepository) : base(archiveRepository, groupRepository)
        {
        }

        public ElectricityMeterRowsView GetRowsView(Guid groupId, Interval interval, ElectricityMeterQuantity[] quantities)
        {
            var archive = _archiveRepository.GetArchive(groupId, (byte)Arch.ElectricityMeter);

            var q = quantities.Select(q => q.ToQuantity()).ToArray();

            var rows = archive.GetRows(new GetArchiveRowsQuery
            {
                Range = interval,
                Quantities = q
            });

            if (rows.Count() == 0)
            {
                return null;
            }

            return new ElectricityMeterRowsView(quantities, rows);
        }
    }
}