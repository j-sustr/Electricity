using System;
using System.Collections.Generic;
using System.Linq;
using Common.Random;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Interfaces.Queries;

namespace Electricity.Infrastructure.DataSource
{
    public class FakeDataSourceTableReader : ITable
    {
        int _seed;

        public FakeDataSourceTableReader(int seed)
        {
            _seed = seed;
        }

        public IEnumerable<Tuple<DateTime, float[]>> GetRows(IGetRowsQuery query)
        {
            int rowLen = query.Quantities.Length;
            var generators = Enumerable.Range(0, rowLen).Select((i) =>
            {
                return new RandomSeries(_seed + i);
            });

            DateTime time = query.Range.Item1;
            TimeSpan interval = new TimeSpan(0, 0, 10);
            while (time < query.Range.Item2)
            {
                var rowValues = generators.Select(g => g.Next()).ToArray();
                yield return Tuple.Create(time, rowValues);
                time += interval;
            }
        }
    }
}