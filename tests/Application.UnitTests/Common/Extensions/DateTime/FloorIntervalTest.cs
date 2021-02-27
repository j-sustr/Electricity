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

        private static TestData[] _yearTestData = new[]{
            new TestData(){
                Source = new DateTime(2021, 1, 10, 20, 30, 40, 50),
                ExpectedResult=new DateTime(2021, 1, 1)
            }
        };

        [Test]
        public void ShouldFloorYear([ValueSource("_yearTestData")] TestData data)
        {
            data.Source.FloorYear().Should().Be(data.ExpectedResult);
        }

        private static TestData[] _monthTestData = new[]{
            new TestData(){
                Source = new DateTime(2021, 3, 10, 20, 30, 40, 50),
                ExpectedResult=new DateTime(2021, 3, 1)
            }
        };

        [Test]
        public void ShouldFloorMonth([ValueSource("_monthTestData")] TestData data)
        {
            data.Source.FloorMonth().Should().Be(data.ExpectedResult);
        }

        private static TestData[] _dayTestData = new[]{
            new TestData(){
                Source = new DateTime(2021,1,2,3,4,5,6),
                ExpectedResult=new DateTime(2021, 1, 2, 0, 0, 0)
            }
        };

        [Test]
        public void ShouldFloorDay([ValueSource("_dayTestData")] TestData data)
        {
            data.Source.FloorDay().Should().Be(data.ExpectedResult);
        }
    }
}