using NUnit.Framework;
using System;
using Electricity.Application.Common.Extensions;
using FluentAssertions;

namespace Electricity.Application.UnitTests.Common.Extensions
{
    public class CeilIntervalTest
    {
        public class TestData
        {
            public DateTime Source { get; set; }
            public DateTime ExpectedResult { get; set; }
        }

        private static TestData[] _yearTestData = new[]{
            new TestData(){
                Source = new DateTime(2021, 1, 10, 20, 30, 40, 50),
                ExpectedResult=new DateTime(2022, 1, 1)
            }
        };

        [Test]
        public void ShouldCeilYear([ValueSource("_yearTestData")] TestData data)
        {
            data.Source.CeilYear().Should().Be(data.ExpectedResult);
        }

        private static TestData[] _monthTestData = new[]{
            new TestData(){
                Source = new DateTime(2021, 3, 10, 20, 30, 40, 50),
                ExpectedResult=new DateTime(2021, 4, 1)
            }
        };

        [Test]
        public void ShouldCeilMonth([ValueSource("_monthTestData")] TestData data)
        {
            data.Source.CeilMonth().Should().Be(data.ExpectedResult);
        }


        private static TestData[] _weekTestData = new[]{
            new TestData(){
                Source = new DateTime(2021,4,19,0,0,0),
                ExpectedResult = new DateTime(2021,4,19,0,0,0)
            },
            new TestData(){
                Source = new DateTime(2021,4,19,0,1,0),
                ExpectedResult = new DateTime(2021,4,26,0,0,0)
            }
        };

        [Test]
        public void ShouldCeilWeek([ValueSource("_weekTestData")] TestData data)
        {
            data.Source.CeilWeek(true).Should().Be(data.ExpectedResult);
        }



        private static TestData[] _dayTestData = new[]{
            new TestData(){
                Source = new DateTime(2021,1,2,3,4,5,6),
                ExpectedResult=new DateTime(2021, 1, 3, 0, 0, 0)
            }
        };

        [Test]
        public void ShouldCeilDay([ValueSource("_dayTestData")] TestData data)
        {
            data.Source.CeilDay().Should().Be(data.ExpectedResult);
        }
    }
}