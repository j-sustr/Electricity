using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using KMB.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public abstract class GetRowsViewQueryBase
    {
        public Guid GroupId { get; set; }
        public Interval Range { get; set; } = Interval.Unbounded;
        public uint Aggregation { get; set; } = 0;
        public EEnergyAggType EnergyAggType { get; set; } = EEnergyAggType.Cumulative;
    }

    public class GetPowerRowsViewQuery : GetRowsViewQueryBase
    {
        public PowerQuantity[] Quantities { get; set; }
    }

    public class GetElectricityMeterRowsViewQuery : GetRowsViewQueryBase
    {
        public ElectricityMeterQuantity[] Quantities { get; set; }
    }

    

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

        public ElectricityMeterRowsView GetElectricityMeterRowsView(GetElectricityMeterRowsViewQuery query)
        {
            var archive = _archiveRepository.GetArchive(query.GroupId, (byte)Arch.ElectricityMeter);

            var quantities = query.Quantities.Select(q => q.ToQuantity()).ToArray();

            var rows = archive.GetRows(new GetArchiveRowsQuery
            {
                Range = query.Range,
                Quantities = quantities,
                Aggregation = query.Aggregation,
                EnergyAggType = query.EnergyAggType
            });

            if (rows.Count() == 0)
            {
                throw new IntervalOutOfRangeException(nameof(query.Range));
            }

            return new ElectricityMeterRowsView(query.Quantities, rows);
        }

        public PowerRowsView GetPowerRowsView(GetPowerRowsViewQuery query)
        {
            var archive = _archiveRepository.GetArchive(query.GroupId, (byte)Arch.Main);

            var quantities = query.Quantities.Select(q => q.ToQuantity()).ToArray();

            var rows = archive.GetRows(new GetArchiveRowsQuery
            {
                Range = query.Range,
                Quantities = quantities,
                Aggregation = query.Aggregation,
                EnergyAggType = query.EnergyAggType
            });

            if (rows.Count() == 0)
            {
                throw new IntervalOutOfRangeException(nameof(query.Range));
            }

            return new PowerRowsView(query.Quantities, rows);
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
