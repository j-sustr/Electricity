using Electricity.Application.Common.Extensions.ITimeSeries;
using Electricity.Application.Common.Models.TimeSeries;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Electricity.Application.UnitTests.Common.Extensions.TimeSeries
{
    internal class ChunkByIntervalTest
    {
        public class TestData
        {
            public ITimeSeries<int> Source { get; set; }
            public IEnumerable<ITimeSeries<int>> ExpectedResult { get; set; }
        }

        private static TestData[] _testData = new[]{
            new TestData(){
                Source = new VariableIntervalTimeSeries<int>(new Tuple<DateTime, int>[]
                {
                    Tuple.Create(new DateTime(2021, 2, 1), 1),
                    Tuple.Create(new DateTime(2021, 2, 10), 1),
                    Tuple.Create(new DateTime(2021, 3, 1), 1),
                    Tuple.Create(new DateTime(2021, 3, 15), 1),
                    Tuple.Create(new DateTime(2021, 5, 20), 1),
                    Tuple.Create(new DateTime(2021, 5, 23), 1),
                }),
                ExpectedResult = new VariableIntervalTimeSeries<int>[]
                {
                    new VariableIntervalTimeSeries<int>(new Tuple<DateTime, int>[] {
                        Tuple.Create(new DateTime(2021, 2, 1), 1),
                        Tuple.Create(new DateTime(2021, 2, 10), 1),
                    }),
                    new VariableIntervalTimeSeries<int>(new Tuple<DateTime, int>[] {
                        Tuple.Create(new DateTime(2021, 3, 1), 1),
                        Tuple.Create(new DateTime(2021, 3, 15), 1),
                    }),
                    null,
                    new VariableIntervalTimeSeries<int>(new Tuple<DateTime, int>[] {
                        Tuple.Create(new DateTime(2021, 5, 20), 1),
                        Tuple.Create(new DateTime(2021, 5, 23), 1),
                    }),
                }
            },
        };

        [Test]
        public void ShouldChunkByMonth([ValueSource("_testData")] TestData data)
        {
            var result = data.Source.ChunkByMonth();

            result.Should().BeEquivalentTo(data.ExpectedResult);
            var enumerator = data.ExpectedResult.GetEnumerator();
            foreach (var series in result)
            {
                enumerator.MoveNext();

                if (enumerator.Current == null)
                {
                    series.Should().BeNull();
                    continue;
                }

                series.Entries().Should().BeEquivalentTo(enumerator.Current.Entries());
            }
        }
    }
}