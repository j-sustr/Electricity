using FluentAssertions;
using NUnit.Framework;
using System;
using Electricity.Application.Common.Extensions;

namespace Electricity.Application.UnitTests.Common.Extensions
{
    internal class FloorIntervalTest
    {
        public class TestData
        {
            public DateTime Source { get; set; }
            public DateTime ExpectedResult { get; set; }
        }

        private static TestData[] _testData = new[]{
            new TestData(){
                Source = new DateTime(2021,1,1,1,1,1,1),
                ExpectedResult=new DateTime(2021, 1, 1)
            }
        };

        [Test]
        public void ShouldFloorMonth([ValueSource("_testData")] TestData data)
        {
            data.Source.FloorMonth().Should().Be(data.ExpectedResult);
        }
    }
}