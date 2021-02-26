using NUnit.Framework;
using System;
using Electricity.Application.Common.Extensions;
using FluentAssertions;

namespace Electricity.Application.UnitTests.Common.Extensions
{
    internal class CountIntervalsToTest
    {
        public class TestData
        {
            public DateTime Source { get; set; }
            public DateTime Target { get; set; }
            public int ExpectedResult { get; set; }
        }

        private static TestData[] _testData = new[]{
            new TestData(){
                Source = new DateTime(2021, 2, 26, 1,1,1,1),
                Target = new DateTime(2022, 1, 1,1,1,1,1),
                ExpectedResult=11
            },
            new TestData(){
                Source = new DateTime(2021, 2, 26, 1,1,1,1),
                Target = new DateTime(2020, 5, 1,1,1,1,1),
                ExpectedResult=-9
            }
        };

        [Test]
        public void ShouldCountMonnths([ValueSource("_testData")] TestData data)
        {
            data.Source.CountMonthsTo(data.Target)
                  .Should().Be(data.ExpectedResult);
        }
    }
}