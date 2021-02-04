using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Random;
using DataSource;
using Electricity.Infrastructure.DataSource.Fake;
using DS = DataSource;

namespace Electricity.Infrastructure.DataSource
{
    public class FakeDataSource : DS.DataSource
    {
        int _seed;

        List<Group> groups = new List<Group>{
            new Group(Guid.NewGuid(), "group-1"),
            new Group(Guid.NewGuid(), "group-2"),
            new Group(Guid.NewGuid(), "group-3"),
        };


        public FakeDataSource(int seed)
        {
            _seed = seed;
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
            if (groups.FindIndex(g => g.ID == groupID) == -1)
            {
                throw new ArgumentException("invalid groupId");
            }

            int rowLen = quantities.Length;
            var generators = Enumerable.Range(0, rowLen).Select((i) =>
            {
                return new RandomSeries(_seed + i);
            }).ToArray();

            foreach (var q in quantities)
            {
                q.Value = new FakePropValueFloat();
            }


            IEnumerable<RowInfo> GenerateRows()
            {
                DateTime time = range.DateMin;
                TimeSpan interval = new TimeSpan(0, 0, 10);
                while (time < range.DateMin)
                {
                    var rowValues = generators.Select(g => g.Next()).ToArray();
                    for (int i = 0; i < generators.Length; i++)
                    {
                        var propValue = quantities[i].Value as FakePropValueFloat;
                        propValue.Value = generators[i].Next();
                    }
                    yield return new RowInfo(time, 0, null);
                    time += interval;
                }
            }

            return new FakeRowCollection(GenerateRows());
        }

        public override List<Group> GetUserGroups(Guid user)
        {
            return groups;
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
    }
}