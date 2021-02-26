using Electricity.Application.Common.Extensions.ITimeSeries;
using Electricity.Application.Common.Models.TimeSeries;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

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
                    Tuple.Create(new DateTime(2021, 1, 1), 1),
                    Tuple.Create(new DateTime(2021, 1, 10), 1),
                    Tuple.Create(new DateTime(2021, 2, 1), 1),
                    Tuple.Create(new DateTime(2021, 2, 15), 1),
                }),
                ExpectedResult = new VariableIntervalTimeSeries<int>[]
                {
                    new VariableIntervalTimeSeries<int>(new Tuple<DateTime, int>[] {
                        Tuple.Create(new DateTime(2021, 1, 1), 1),
                        Tuple.Create(new DateTime(2021, 1, 10), 1),
                    }),
                    new VariableIntervalTimeSeries<int>(new Tuple<DateTime, int>[] {
                        Tuple.Create(new DateTime(2021, 2, 1), 1),
                        Tuple.Create(new DateTime(2021, 2, 15), 1),
                    })
                }
            },
        };

        [Test]
        public void ShouldChunkByMonth([ValueSource("_testData")] TestData data)
        {
            data.Source.ChunkByMonth().Should().BeEquivalentTo(data.ExpectedResult);
        }
    }
}