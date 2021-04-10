using KMB.DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public class PowerService : ArchiveRepositoryService
    {
        public PowerService(
            IArchiveRepository archiveRepository,
            IGroupRepository groupRepository): base(archiveRepository, groupRepository)
        {
        }

        public PowerRowsView GetRowsView(Guid groupId, Interval interval, PowerQuantity[] quantities)
        {
            var table = _archiveRepository.GetArchive(groupId, (byte)Arch.Main);

            var q = quantities.Select(q => q.ToQuantity()).ToArray();

            var rows = table.GetRows(new Models.Queries.GetArchiveRowsQuery
            {
                Interval = interval,
                Quantities = q
            });

            if (rows.Count() == 0)
            {
                return null;
            }

            return new PowerRowsView(quantities, rows);
        }
    }
}