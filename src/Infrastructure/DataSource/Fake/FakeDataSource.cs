using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KMB.DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Utils;
using Electricity.Infrastructure.DataSource.Fake;
using Electricity.Infrastructure.DataSource.Util;
using Electricity.Application.Common.Extensions;

namespace Electricity.Infrastructure.DataSource
{
    public class FakeDataSource : KMB.DataSource.DataSource
    {
        private int _seed;

        private FakeUserData _currentUser = null;
        public FakeUserData[] Users { get; set; }


        public FakeDataSource(int seed)
        {
            _seed = seed;
        }

        public override void Dispose()
        {
            
        }

        private int GuidToHash(Guid guid)
        {
            return StringToHash(guid.ToString());
        }

        private int DateRangeToHash(DateRange range)
        {
            var min = range.DateMin.ToString();
            var max = range.DateMax.ToString();
            return StringToHash(min) + StringToHash(max);
        }

        private int QuantityToHash(Quantity quant)
        {
            return StringToHash(quant.PropName);
        }

        private int StringToHash(string str)
        {
            int value = 0;
            var chars = str.ToCharArray();

            foreach (var c in chars)
            {
                value += c;
            }

            return value;
        }

        public override IDisposable NewConnection()
        {
            return null;
        }

        public override IDisposable BeginTransaction(IDisposable connection)
        {
            return null;
        }

        public override void CommitTransaction(IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override void RollbackTransaction(IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override Guid Login(string ENVISUser, string ENVISPassword, IDisposable connection, IDisposable transaction)
        {
            var user = Array.Find(Users, (user) => user.Username == ENVISUser);
            if (user == null)
            {
                throw new Exception("Bad User");
            }
            
            _currentUser = user;

            return user.UserId;
        }

        public override GroupInfo GetGroupInfos(Guid ID, InfoFilter filter, IDisposable connection, IDisposable transaction)
        {
            var groupTree = _currentUser.GroupTree;

            if (filter.IDisGroup)
            {
                if (filter.RecurseSubgroups)
                {
                    throw new NotImplementedException();
                }

                return groupTree.Find(ID);
            }

            if (!filter.RecurseSubgroups)
            {
                throw new NotImplementedException();
            }

            if (filter.infoType != ArchiveInfoType.All)
            {
                throw new NotImplementedException();
            }

            if (filter.range != null)
            {
                throw new NotImplementedException();
            }

            return GroupInfoUtil.CloneGroupInfo(groupTree);
        }

        public override List<Group> GetUserGroups(Guid user, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override List<Group> GetGroups(Guid parent, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override Quantity[] GetQuantities(Guid GroupID, byte arch, DateRange range, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override RowCollection GetRows(Guid groupID, byte arch, DateRange range, Quantity[] quantities, uint aggregation, IDisposable connection, IDisposable transaction, EEnergyAggType energyAggType = KMB.DataSource.EEnergyAggType.Cumulative)
        {
            var groupTree = _currentUser.GroupTree;

            var groupArray = groupTree.GetUserRecordGroupInfos();
            var groupIndex = Array.FindIndex(groupArray, g => g.ID == groupID);
            if (groupIndex == -1)
            {
                throw new ArgumentException("invalid groupId");
            }
            var group = groupArray[groupIndex];

            bool cumulative = arch == (byte)Arch.ElectricityMeter;

            var generators = quantities.Select((quant) =>
            {
                int groupIdHash = GuidToHash(groupID);
                int rangeHash = range != null ? DateRangeToHash(range) : 0;
                int quanityHash = QuantityToHash(quant);
                var hashSeed = groupIdHash + rangeHash + quanityHash;
                var g = new RandomSeries(0, _seed + hashSeed);
                g.Cumulative = cumulative;
                return g;
            }).ToArray();

            foreach (var q in quantities)
            {
                q.Value = new FakePropValueFloat();
            }

            var archive = group.Archives[arch];
            var interval = Interval.FromDateRange(archive.Range);

            if (range != null)
            {
                interval = interval.GetOverlap(Interval.FromDateRange(range));

                if (interval == null)
                {
                    return new FakeRowCollection();
                }
            }

            IEnumerable<RowInfo> GenerateRows()
            {
                DateTime time = interval.Start ?? archive.Range.DateMin;
                TimeSpan duration = new TimeSpan(0, 0, 10);
                while (time < interval.End)
                {
                    var rowValues = generators.Select(g => g.Next()).ToArray();
                    for (int i = 0; i < generators.Length; i++)
                    {
                        var propValue = quantities[i].Value as FakePropValueFloat;
                        propValue.Value = generators[i].Next();
                    }
                    yield return new RowInfo(time, 0, null);
                    time += duration;
                }
            }

            return new FakeRowCollection(GenerateRows());
        }

        protected override IList<UniConfig> GetConfs(int RecID, DateRange range, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        protected override IList<UniArchiveBinPack> GetBinPacks(int RecID, byte arch, DateRange range, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        protected override Stream GetStreamToRead(int RecID, byte arch, DateTime keyTime, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        protected override UniArchiveBinPack BinPackToWrite(int recID, byte archID, DateTime time, ref DateTime curIntStart, ref DateTime curIntEnd, UniArchiveDefinition uad, uint period, IDisposable connection, IDisposable transaction, bool directFS)
        {
            throw new NotImplementedException();
        }

        protected override void CommitBinPack(int recID, byte archID, UniArchiveBinPack ua, DateTime time, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override int SaveRecord(SmpMeasNameDB record, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override void SaveArchiveDefinition(UniArchiveDefinition uad, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override void SaveConfig(UniConfig cfg, int recID, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override List<SmpMeasNameDB> GetRecords(IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}