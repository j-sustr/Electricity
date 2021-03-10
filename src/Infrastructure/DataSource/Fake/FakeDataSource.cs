using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Utils;
using Electricity.Infrastructure.DataSource.Fake;

using DS = DataSource;

namespace Electricity.Infrastructure.DataSource
{
    public class FakeDataSource : DS.DataSource
    {
        private int _seed;

        private BoundedInterval _interval { get; set; }

        public List<Group> Groups { get; set; }

        public FakeDataSource(int seed, BoundedInterval interval)
        {
            _seed = seed;
            _interval = interval;
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override List<Group> GetGroups(Guid parent)
        {
            throw new NotImplementedException();
        }

        public override Quantity[] GetQuantities(Guid groupID, byte arch, DateRange range)
        {
            throw new NotImplementedException();
        }

        public override RowCollection GetRows(Guid groupID, byte arch, DateRange range, Quantity[] quantities, uint aggregation, EEnergyAggType energyAggType = EEnergyAggType.Cumulative)
        {
            if (Groups.FindIndex(g => g.ID == groupID) == -1)
            {
                throw new ArgumentException("invalid groupId");
            }

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

            var interval = _interval.ToInterval();

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
                DateTime time = interval.Start ?? _interval.Start;
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

        public override List<Group> GetUserGroups(Guid user)
        {
            return Groups;
        }

        public override Guid Login(string ENVISUser, string ENVISPassword)
        {
            throw new NotImplementedException();
        }

        protected override IList<UniArchiveBinPack> GetBinPacks(int RecID, byte arch, DateRange range)
        {
            throw new NotImplementedException();
        }

        protected override IList<UniConfig> GetConfs(int RecID, DateRange range)
        {
            throw new NotImplementedException();
        }

        protected override Stream GetStreamToRead(int RecID, byte arch, DateTime keyTime)
        {
            throw new NotImplementedException();
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
    }
}