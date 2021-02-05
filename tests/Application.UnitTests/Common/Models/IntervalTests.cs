using System;
using Electricity.Application.Common.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Electricity.Application.UnitTests.Common.Models
{
    public class IntervalTests
    {
        public class OverlapTestData
        {
            public Interval IntA { get; set; }
            public Interval IntB { get; set; }
            public Interval ExpectedResult { get; set; }
        }

        private static OverlapTestData[] _overlapTestData = new[]{
            new OverlapTestData(){
                IntA = new Interval(new DateTime(2021, 1, 1), new DateTime(2021, 1, 20)),
                IntB = new Interval(new DateTime(2021, 1, 15), new DateTime(2021, 1, 31)),
                ExpectedResult = new Interval(new DateTime(2021, 1, 15), new DateTime(2021, 1, 20))
            },
            new OverlapTestData(){
                IntA = new Interval(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10)),
                IntB = new Interval(new DateTime(2021, 1, 15), new DateTime(2021, 1, 31)),
                ExpectedResult = null
            }
        };

        [Test]
        public void OverlapTest([ValueSource("_overlapTestData")] OverlapTestData testData)
        {
            testData.IntA.GetOverlap(testData.IntB).Should().BeEquivalentTo(testData.ExpectedResult);
        }

        [Test]
        public void EqualsTest()
        {
            var intA = new Interval(new DateTime(2021, 1, 1), new DateTime(2021, 1, 20));
            var intB = new Interval(new DateTime(2021, 1, 1), new DateTime(2021, 1, 20));

            var result = intA.Equals(intB);

            result.Should().BeTrue();
        }
    }
}