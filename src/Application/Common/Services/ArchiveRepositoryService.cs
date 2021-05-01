using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Dtos;
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

    public class GetMainRowsViewQuery : GetRowsViewQueryBase
    {
        public MainQuantity[] Quantities { get; set; }
    }

    public class GetElectricityMeterRowsViewQuery : GetRowsViewQueryBase
    {
        public ElectricityMeterQuantity[] Quantities { get; set; }
    }

    public class ArchiveQueryRecord
    {
        public string GroupId { get; set; }
        public Arch Arch { get; set; }
        public IntervalDto Range { get; set; }
        public Tuple<string, string>[] Quantities { get; set; }
        public uint Aggregation { get; set; }
        public EEnergyAggType EnergyAggType { get; set; }

    }

    public class ArchiveRepositoryService
    {

        private static List<ArchiveQueryRecord> _queryRecords = new List<ArchiveQueryRecord>();

        protected readonly IArchiveRepository _archiveRepository;
        protected readonly IGroupRepository _groupRepository;

        public ArchiveRepositoryService(
            IArchiveRepository archiveRepository,
            IGroupRepository groupRepository)
        {
            _archiveRepository = archiveRepository;
            _groupRepository = groupRepository;
        }

        public MainRowsView GetMainRowsView(GetMainRowsViewQuery query)
        {
            AddArchiveQueryRecord(query, Arch.Main);

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

            return new MainRowsView(query.Quantities, rows);
        }

        public ElectricityMeterRowsView GetElectricityMeterRowsView(GetElectricityMeterRowsViewQuery query)
        {
            AddArchiveQueryRecord(query, Arch.ElectricityMeter);

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

            return new ElectricityMeterRowsView(query.Quantities, rows, query.EnergyAggType);
        }

        public bool HasArchive(Guid groupId, Arch arch)
        {
            var groupInfo = _groupRepository.GetGroupInfo(groupId.ToString());
            try
            {
                return groupInfo.Archives[(int)arch] != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Interval GetRangeOverlapWithMain(Guid groupId, Interval interval)
        {
            return GetRangeOverlapWithArchives(groupId, interval, new Arch[] {
                Arch.Main
            });
        }

        public Interval GetRangeOverlapWithElectrityMeter(Guid groupId, Interval interval)
        {
            return GetRangeOverlapWithArchives(groupId, interval, new Arch[] { 
                Arch.Main, Arch.ElectricityMeter
            });
        }

        public Interval GetRangeOverlapWithArchives(Guid groupId, Interval interval, Arch[] archives)
        {
            var overlap = interval;

            foreach (var arch in archives)
            {
                var range = GetRangeForArchive(groupId, arch);

                overlap = overlap.GetOverlap(range);
            }

            return overlap;
        }

        public Interval GetRangeForArchive(Guid groupId, Arch arch)
        {
            var groupInfo = _groupRepository.GetGroupInfo(groupId.ToString());
            DateRange range = null;
            try
            {
                range = groupInfo.Archives[(int)arch].Range;
            }
            catch (Exception ex)
            {
                return null;
            }

            if (range == null) return null;

            return Interval.FromDateRange(range);
        }

        public bool HasDataOnRange(Guid groupId, Interval interval, Arch arch)
        {
            var groupInfo = _groupRepository.GetGroupInfo(groupId.ToString());
            ArchiveInfo archInfo = null;
            try
            {
                archInfo = groupInfo.Archives[(int)arch];
            }
            catch (Exception ex)
            {
                return false;
            }

            if (archInfo == null)
                return false;

            var range = Interval.FromDateRange(archInfo.Range);

            var overlap = interval.GetOverlap(range);
            if (overlap == null)
            {
                return false;
            }
            return true;
        }


        // --- DEBUG ---

        public ArchiveQueryRecord[] GetQueryRecords()
        {
            var records = _queryRecords.ToArray();
            _queryRecords.Clear();
            return records;
        }

        private void AddArchiveQueryRecord(GetRowsViewQueryBase query, Arch arch)
        {
            Tuple<string, string>[] quantities = null;
            if (query is GetMainRowsViewQuery)
            {
                quantities = ((GetMainRowsViewQuery)query).Quantities
                    .Select(q => {
                        var name1 = q.Type.ToString() + "," + q.Phase.ToString();
                        var name2 = q.ToQuantity().ToString();
                        return Tuple.Create(name1, name2);
                    })
                    .ToArray();
            }
            else if (query is GetElectricityMeterRowsViewQuery)
            {
                quantities = ((GetElectricityMeterRowsViewQuery)query).Quantities
                    .Select(q => {
                        var name1 = q.Type.ToString() + "," + q.Phase.ToString();
                        var name2 = q.ToQuantity().ToString();
                        return Tuple.Create(name1, name2);
                    })
                    .ToArray();
            }

            IntervalDto range;
            range = new IntervalDto
            {
                Start = query.Range.Start,
                End = query.Range.End,
                IsInfinite = query.Range.IsInfinite
            };

            _queryRecords.Add(new ArchiveQueryRecord
            {
                GroupId = query.GroupId.ToString(),
                Arch = arch,
                Range = range,
                Quantities = quantities,
                Aggregation = query.Aggregation,
                EnergyAggType = query.EnergyAggType,
            });
        }
    }
}
